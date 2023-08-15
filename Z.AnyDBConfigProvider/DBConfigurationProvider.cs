using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Text.Json;

namespace Z.AnyDBConfigProvider
{
    public class DBConfigurationProvider : ConfigurationProvider, IDisposable
    {
        private DBConfigOptions options;
        //allow multi reading and single writing
        private ReaderWriterLockSlim lockObj = new ReaderWriterLockSlim();
        private bool isDisposed = false;

        public DBConfigurationProvider(DBConfigOptions options)
        {
            this.options = options;
            TimeSpan interval = TimeSpan.FromSeconds(3);
            if (options.ReloadInterval != null)
            {
                interval = options.ReloadInterval.Value;
            }
            if (options.ReloadOnChange)
            {
                //进入队列，当有可用线程时执行方法
                ThreadPool.QueueUserWorkItem(obj =>
                {
                    while (!isDisposed)
                    {
                        Load();
                        Thread.Sleep(interval);//将当前线程挂起指定的时间
                    }
                });
            }
        }

        public void Dispose()
        {
            this.isDisposed = true;
        }

        /// <summary>
        /// 线程安全的GetChildKeys
        /// 返回此提供程序拥有的键列表
        /// </summary>
        /// <param name="earlierKeys"></param>
        /// <param name="parentPath"></param>
        /// <returns></returns>
        public override IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string? parentPath)
        {
            lockObj.EnterReadLock();
            try
            {
                return base.GetChildKeys(earlierKeys, parentPath);
            }
            finally
            {
                lockObj.ExitReadLock();
            }

        }

        /// <summary>
        /// 线程安全的TryGet
        /// 获取当前实例的 Type。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TryGet(string key, out string? value)
        {
            lockObj.EnterReadLock();
            try
            {
                return base.TryGet(key, out value);
            }
            finally
            {
                lockObj.ExitReadLock();
            }
        }

        /// <summary>
        /// 原Load()加载（或重载）此提供程序的数据。
        /// 对原有Load进行扩展，对原先的Data进行克隆保存到cloneData，随后对数据库进行读取给Data重新赋值，并判断cloneData与Data是否一致，不一致则调用OnReload
        /// </summary>
        public override void Load()
        {
            base.Load();
            IDictionary<string, string> clonedData = null;
            try
            {
                lockObj.EnterWriteLock();
                clonedData = Data.Clone();
                string tableName = options.TableName;
                Data.Clear();
                using (var conn = options.CreateDbConnection())
                {
                    conn.Open();
                    DoLoad(tableName, conn);
                }
            }
            catch (DbException)
            {
                //if DbException is thrown, restore to the original data.
                this.Data = clonedData;
                throw;
            }
            finally
            {
                lockObj.ExitWriteLock();
            }

            //OnReload cannot be between EnterWriteLock and ExitWriteLock, or "A read lock may not be acquired with the write lock held in this mode" will be thrown.
            if (Helper.IsChanged(clonedData, Data))
            {
                OnReload();//触发重载更改令牌并创建一个新令牌
            }
        }

        /// <summary>
        /// 在指定的数据库连接对象与表名中查询name，value，并转化为字符串存在当前对象的键值对Data
        /// </summary>
        /// <param name="tableName">数据库表名</param>
        /// <param name="conn">数据库连接对象</param>
        private void DoLoad(string tableName, IDbConnection conn)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = $"select Name,Value from {tableName} where Id in(select Max(Id) from {tableName} group by Name)";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string name = reader.GetString(0);
                        string value = reader.GetString(1);
                        if (value == null)
                        {
                            this.Data[name] = value;
                            continue;
                        }
                        value = value.Trim();
                        //if the value is like [...] or {} , it may be a json array value or json object value,
                        //so try to parse it as json
                        if (value.StartsWith("[") && value.EndsWith("]") || value.StartsWith("{") && value.EndsWith("}"))
                        {
                            TryLoadAsJson(name, value);
                        }
                        else
                        {
                            this.Data[name] = value;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 将json元素转化为字符串被存入到当前对象的Data之中
        /// </summary>
        /// <param name="name"></param>
        /// <param name="jsonRoot"></param>
        private void LoadJsonElement(string name, JsonElement jsonRoot)
        {
            if (jsonRoot.ValueKind == JsonValueKind.Array)
            {
                int index = 0;
                foreach (var item in jsonRoot.EnumerateArray())
                {
                    //https://andrewlock.net/creating-a-custom-iconfigurationprovider-in-asp-net-core-to-parse-yaml/
                    //parse as "a:b:0"="hello";"a:b:1"="world"
                    string path = name + ConfigurationPath.KeyDelimiter + index;
                    LoadJsonElement(path, item);
                    index++;
                }
            }
            else if (jsonRoot.ValueKind == JsonValueKind.Object)
            {
                foreach (var jsonObj in jsonRoot.EnumerateObject())
                {
                    string pathOfObj = name + ConfigurationPath.KeyDelimiter + jsonObj.Name;
                    LoadJsonElement(pathOfObj, jsonObj.Value);
                }
            }
            else
            {
                //if it is not json array or object, parse it as plain string value
                this.Data[name] = jsonRoot.GetValueForConfig();
            }
        }

        /// <summary>
        /// 尝试将json元素转化为字符串被存入到当前对象的Data之中，失败时抛出JsonException
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        private void TryLoadAsJson(string name, string value)
        {
            var jsonOptions = new JsonDocumentOptions
            {
                AllowTrailingCommas = true,
                CommentHandling = JsonCommentHandling.Skip
            };
            try
            {
                var jsonRoot = JsonDocument.Parse(value, jsonOptions).RootElement;
                LoadJsonElement(name, jsonRoot);
            }
            catch (JsonException ex)
            {
                //if it is not valid json, parse it as plain string value
                this.Data[name] = value;
                Debug.WriteLine($"When trying to parse {value} as json object, exception was thrown. {ex}");
            }
        }
    }
}

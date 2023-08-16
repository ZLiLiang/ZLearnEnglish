using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z.FileService.SDK.NETCore
{
    public record FileExistsResponse(bool IsExists, Uri? Url);
}

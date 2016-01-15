/**************************************************************************
Natteravnenes Intranet (c) by Dan Taxbøl

Natteravnenes Intranet is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

You should have received a copy of the license along with this
work.  If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NR.Models
{
    public class DirectoryModel
    {
        public string Name { get; set; }
        public DirectoryModel Parent { get; set;}
        public List<DirectoryModel> Children { get; set;}
        public List<FileModel> Files { get; set; }
    }


    public class FileModel
    {
        public string name {get; set;}
        public string type { get; set; }
        public string Comment { get; set; }

    }

    public class FulexTree
    {
        public string name { get; set; }
        public string type { get; set; }
        public string url { get; set; }
        public FulexdataAttributes dataAttributes { get; set; }
    }

    public class FulexdataAttributes
    {
        public string id { get; set;  }
        public bool children { get; set; }
    }

}
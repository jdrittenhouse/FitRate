using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitRate.Core.Models
{
    public class FirestoreDocument<T>
    {
        public string name { get; set; }
        public T fields { get; set; }
        public DateTime createTime { get; set; }
        public DateTime updateTime { get; set; }
    }

}

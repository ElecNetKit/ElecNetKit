using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.Serialization;

namespace ElecNetKitExplorer
{
    [Serializable]
    public class Package : IDeserializationCallback
    {
        public String Name { private set; get; }
        public Version Version { private set; get; }
        public DateTime? DateModified { private set; get; }
        public String Path { private set; get; }

        [NonSerialized]
        bool _Error;

        public bool Error { get { return _Error; } }

        public Package(String Path)
        {
            this.Path = Path;
            TryFillDetails();
        }

        protected Package() { }

        protected void TryFillDetails()
        {
            this._Error = false;
            try
            {
                var assemblyName = AssemblyName.GetAssemblyName(this.Path);
                this.Name = assemblyName.Name;
                this.Version = assemblyName.Version;
                var fileInfo = new System.IO.FileInfo(this.Path);
                this.DateModified = fileInfo.LastWriteTime;
            }
            catch
            {
                this._Error = true;
            }
        }

        public void OnDeserialization(object sender)
        {
            TryFillDetails();
        }
    }
}

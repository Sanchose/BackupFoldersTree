using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Data.HashFunction.xxHash;
using System.Security.Cryptography;

namespace BackupBot_new
{
    class FileFolderNode
    {
        private List<FileFolderNode> Children = new List<FileFolderNode>();
        public LinkedList<FileFolderNode> subNodes = new LinkedList<FileFolderNode>();

        //public FileInfo fileInfo = null;//remove
        //public DirectoryInfo directoryInfo = null; //remove

        //use
        public string path;
        public bool isDirectory;
        public DateTime lastWritetime;
        

        //Append Constructor to read Node Tree from XML-File "Bsp: public FileFodlerNode(Xml reader)"



        public FileFolderNode(XmlReader reader)
        {
            
            reader.ReadStartElement("node");
            reader.ReadToFollowing("type");
            isDirectory = reader.ReadElementContentAsString() == "Folder";
            reader.ReadToFollowing("path");
            path = reader.ReadElementContentAsString();
            Console.WriteLine("PATH:" + path);
            reader.ReadToFollowing("lastChanged");
            string test1 = reader.ReadElementContentAsString();
             lastWritetime = DateTime.Parse(test1);

            if (reader.ReadToFollowing("children"))
            {
                while (reader.ReadToFollowing("node"))
                    Children.Add(new FileFolderNode(reader.ReadSubtree()));
            }
        }

        FileFolderNode(FileInfo fileInfo)
        {
            path = fileInfo.FullName;
            isDirectory = false;
            lastWritetime = fileInfo.LastWriteTime;
        }
        FileFolderNode(DirectoryInfo directoryInfo)
        {
            path = directoryInfo.FullName;
            isDirectory = true;
            lastWritetime = directoryInfo.LastWriteTime;

            foreach (DirectoryInfo d in directoryInfo.GetDirectories())
            {
                Children.Add(new FileFolderNode(d));
            }
            foreach (FileInfo f in directoryInfo.GetFiles())
            {
                Children.Add(new FileFolderNode(f));
            }
        }
        public FileFolderNode(string path)
        {
            if (File.Exists(path))
            {
                isDirectory = false;
                this.path = path;
            }
            else if (Directory.Exists(path))
            {
                isDirectory = true;
                this.path = path;

                DirectoryInfo directoryInfo = new DirectoryInfo(path);

                foreach (FileInfo f in directoryInfo.GetFiles())
                {
                    Children.Add(new FileFolderNode(f));
                }
                foreach (DirectoryInfo d in directoryInfo.GetDirectories())
                {
                    Children.Add(new FileFolderNode(d));
                }

            }
        }

        void AppendNode2Xml(XmlWriter writer)
        {
            
            if (!isDirectory)
            {

                IxxHash hasher = xxHashFactory.Instance.Create();
                var hash = hasher.ComputeHash(new BufferedStream(File.OpenRead(path)));
                string hashString = hash.AsBase64String();



                writer.WriteStartElement("hash");
                writer.WriteString(hashString);
                writer.WriteEndElement();
            }

            /*
            byte[] tmpSource;
            byte[] tmpHash;
            byte[] tmpNewHash;
            bool isEqual = false;
            int i = 0;

            //FileFolderNode node = new FileFolderNode(hashString);
            tmpSource = ASCIIEncoding.ASCII.GetBytes(hashString);

            tmpHash = new MD5CryptoServiceProvider().ComputeHash(tmpSource);
            tmpSource = ASCIIEncoding.ASCII.GetBytes(hashString);

            tmpNewHash = new MD5CryptoServiceProvider().ComputeHash(tmpSource);

            if(tmpHash.Length == tmpHash.Length)
            {
                while((i < tmpNewHash.Length) && (tmpNewHash[i] == tmpHash[i]))
                    {
                    i++;
                }
                if( i == tmpHash.Length)
                {
                    isEqual = true;
                }
            }
            if(isEqual)
            {
                Console.WriteLine("Es sind die selben Werte!");

            }
            else
            {
                Console.WriteLine("Es wurde etwas verändert!");
            }

            */
             

            writer.WriteStartElement("node");

            writer.WriteStartElement("type");
            if (this.isDirectory) writer.WriteString("Folder");
            else writer.WriteString("File");
            writer.WriteEndElement();

            writer.WriteStartElement("path");
            writer.WriteString(path);
            writer.WriteEndElement();

            writer.WriteStartElement("lastChanged");
            writer.WriteString(lastWritetime.ToString());
            writer.WriteEndElement();
            
            if (Children.Count > 0)
            {
                writer.WriteStartElement("children");

                foreach (var c in Children)
                {
                    c.AppendNode2Xml(writer);

                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        public bool SaveToFile(string path)
        {
            var sts = new XmlWriterSettings()
            {
                Indent = true,
            };

            var writer = XmlWriter.Create(path, sts);

            writer.WriteStartDocument();

            AppendNode2Xml(writer);

            writer.WriteEndDocument();
            writer.Close();

            return true;
        }
    }
}

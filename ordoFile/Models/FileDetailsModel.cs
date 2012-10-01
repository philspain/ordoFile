using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ordoFile.Models
{
    struct FileDetailsModel
    {
         string filePath, newFilePath, fileName, fileType, destDir;
         long fileSize;

        public FileDetailsModel(string filePath, string newFilePath, string fileName, string fileType, string destDir, long fileSize)
        {
            this.filePath = filePath;
            this.newFilePath = newFilePath;
            this.destDir = destDir;
            this.fileName = fileName;
            this.fileType = fileType;
            this.fileSize = fileSize;
        }

        public string FilePath
        {
            get {return this.filePath;}
            set { this.filePath = value; }
        }

        public string NewFilePath
        {
            get { return this.newFilePath; }
            set { this.newFilePath = value; }
        }

        public string FileName
        {
            get { return this.fileName; }
            set { this.fileName = value; }
        }

        public string FileType
        {
            get { return this.fileType; }
            set { this.fileType = value; }
        }

        public string DestDir
        {
            get { return this.destDir; }
            set { this.destDir = value; }
        }

        public long FileSize
        {
            get { return this.fileSize; }
            set { this.fileSize = value; }
        }
    }
}

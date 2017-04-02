//Pranab Baidya
//University of Washington, Tacoma
// Project: Azure Blob Upload contents of a file

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzUploadApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string connString = ConfigurationManager.ConnectionStrings["FaceBoothLA"].ConnectionString;
            string localFolder = ConfigurationManager.AppSettings["sourceFolder"];
            string destContainer = ConfigurationManager.AppSettings["destContainer"];

            //get a reference to the storage account
            Console.WriteLine(@"connecting to storage account");
            CloudStorageAccount sa = CloudStorageAccount.Parse(connString);
            CloudBlobClient bc = sa.CreateCloudBlobClient();

            //Get a reference to the container (creating it if necessary)
            Console.WriteLine(@"Getting reference to container");
            CloudBlobContainer container = bc.GetContainerReference(destContainer);


            //Create this container if it doesn't exist
            container.CreateIfNotExists();

            //Upload files
            string[] fileEntries = Directory.GetFiles(localFolder);
            foreach (string filePath in fileEntries)
            {
                //get the date use with the key
                string key = DateTime.UtcNow.ToString("yyyy-MM-dd-HH:mm:ss") + "-" + Path.GetFileName(filePath);

                UploadBlob(container, key, filePath, true);

            }//for

            Console.WriteLine(@"Upload complete. Press key to exit ... ");
            Console.ReadKey();
        }

        static void UploadBlob(CloudBlobContainer container, string key, string fileName, bool deleteAfter)
        {
            Console.WriteLine(@"Uploading file to container: key = " + key + "source file = " + fileName);

            //Get a blob refernce to write this file to
            CloudBlockBlob b = container.GetBlockBlobReference(key);

            //write the file
            using (var fs = System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                b.UploadFromStream(fs);
            }

            //if delete of file is requested, then its done
            if (deleteAfter)
                File.Delete(fileName);

            //throw new NotImplementedException();
        }
    }
}

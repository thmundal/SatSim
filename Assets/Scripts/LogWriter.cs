using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;

public class LogWriter
{
    private string filename;
    public LogWriter(string filename)
    {
        this.filename = filename;
    }

    public void WriteLine(string line)
    {
        Task.Run(() =>
        {
            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(@filename, true))
            {
                file.WriteLine(line);
            }
        });
    }

    public void Write(string data)
    {
        Task.Run(() =>
        {
            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(@filename, true))
            {
                file.Write(data);
            }
        });
    }

    public long Length()
    {
        FileInfo fileInfo = new FileInfo(filename);

        if(fileInfo.Exists)
        {
            return fileInfo.Length;
        }

        return 0;
    }
}

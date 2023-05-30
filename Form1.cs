using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rotateimages
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void btnProcess_Click(object sender, EventArgs e)
        {
           
                var folderDialog = new FolderBrowserDialog();
                var result = folderDialog.ShowDialog();

             
                if (result == DialogResult.OK)
                {
                    var sourceFolder = folderDialog.SelectedPath;
                    var targetFolder = Path.Combine(sourceFolder, "RotatedImages");

                    if (!Directory.Exists(targetFolder))
                    {
                        Directory.CreateDirectory(targetFolder);
                    }

                    var reportFile = Path.Combine(sourceFolder, "report.txt");

                    using (var writer = new StreamWriter(reportFile))
                    {
                        var files = Directory.GetFiles(sourceFolder, "*.jpg");
                        var tasks = new List<Task>();
                        var threads =new List<Thread>();
                        var startTime = DateTime.Now;


                    //РЕШЕНИЕ С ПОМОЩЬЮ THREAD

                        foreach (var file in files)
                        {
                            var thread = new Thread(() =>
                            {
                                var image = Image.FromFile(file);
                                image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                                var targetFile = Path.Combine(targetFolder, Path.GetFileName(file));
                                image.Save(targetFile);
                                var elapsedTime = (DateTime.Now - startTime).TotalMilliseconds;
                                writer.WriteLine($"{file}\n\t Thread отработал за{elapsedTime:0.000} ms");
                            });

                            thread.Start();
                            threads.Add(thread);

                        }

                        foreach (var thread in threads)
                        {
                            thread.Join();
                        }




                       // Решение с помощью Parallel
                        Parallel.ForEach(files, file =>
                        {
                            var image = Image.FromFile(file);
                            image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            var targetFile = Path.Combine(targetFolder, Path.GetFileName(file));
                            image.Save(targetFile);
                            var elapsedTime = (DateTime.Now - startTime).TotalMilliseconds;
                            writer.WriteLine($"{file}\n\t Parallel отработал за{elapsedTime:0.000} ms");

                        });



                        //Решение с помощью TASK
                        foreach (var file in files)
                        {
                            var task = Task.Run(() =>
                            {
                                var image = Image.FromFile(file);
                                image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                                var targetFile = Path.Combine(targetFolder, Path.GetFileName(file));
                                image.Save(targetFile);
                                var elapsedTime = (DateTime.Now - startTime).TotalMilliseconds;
                                writer.WriteLine($"{file}\n\t Task отработал за {elapsedTime:0.000}  ms");
                            });

                            tasks.Add(task);
                        }

                        await Task.WhenAll(tasks);
                    }

                        MessageBox.Show("Process completed!");
                        // читаем содержимое файла report.txt
                        string[] lines = File.ReadAllLines(reportFile);

                        // выводим результат в textBox
                        textBox1.Lines = lines;
                }

            
        }
    }
}

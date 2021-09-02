using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using itk.simple;

namespace ConsoleApp5
{
    class Program
    {
        struct tup_string
        {
            public tup_string(string x, string y)
            {
                this.x = x;
                this.y = y;
            }
            public string x;
            public string y;

        }

        static void Main(string[] args)
        {
            var watch = new Stopwatch();
            watch.Start();

            ImageFileReader imageFileReader = new ImageFileReader();
            imageFileReader.SetFileName("C:\\Users\\m3xpgag\\source\\repos\\" +
                "ConsoleApp5\\ConsoleApp5\\sample.mhd");
            imageFileReader.SetOutputPixelType(PixelIDValueEnum.sitkFloat32);
            imageFileReader.SetImageIO("MetaImageIO");
            Image image=imageFileReader.Execute();


            ImageFileWriter imageFileWriter = new ImageFileWriter();
            imageFileWriter.KeepOriginalImageUIDOn();
           

          //  \\|//
          //  //|\\

            String modification_time= Getcurrentime();
            String modification_date= Getcurrendate();

            VectorDouble direction_Vec = image.GetDirection();
            double[] direction = direction_Vec.ToArray();
            //Console.WriteLine(conc(direction));

            String rescale_slope = "0.001";

            var series_tag_values = new ArrayList()
                {
                 new tup_string("0008|0031", modification_time),
                new tup_string("0008|0021", modification_date),
                 new tup_string("0008|0008", "DERIVED\\SECONDARY"),
                new tup_string("0020|000e", "1.2.826.0.1.3680043.2.1125."+ modification_date + ".1" + modification_time),
                new tup_string("0020|0037",conc(direction)),
                new tup_string("0008|103e", "Created-SimpleITK"),


                    new tup_string("0028|1053", rescale_slope), 
                new tup_string("0028|1052", "0"),
                new tup_string("0028|0100", "16"),
                new tup_string("0028|0101", "16"),
                     new tup_string("0028|0102", "15"), 
                new tup_string("0028|0103", "1") 
                };

            for(uint i = 0; i < image.GetDepth(); i++)
            {
                Write_slice(series_tag_values, image, i, imageFileWriter);
               // Console.WriteLine(i);
            }
            watch.Stop();

            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
            Console.ReadKey();
        }

        static void Write_slice(ArrayList series_tag_values, Image im3d,uint depth, ImageFileWriter ifw)
        {
            
            Image im2d = getnth2dsliceof3dimage(im3d, depth);
            foreach (tup_string tup in series_tag_values.ToArray()) {
                im2d.SetMetaData(tup.x, tup.y);
            }

            im2d.SetMetaData("0008|0012", Getcurrendate());

            im2d.SetMetaData("0008|0012", Getcurrentime());

            im2d.SetMetaData("0008|0060", "CT");

           int i = Convert.ToInt32(depth);
            Int64 tt = Convert.ToInt64(i);

            VectorDouble drr=
            im3d.TransformIndexToPhysicalPoint(new VectorInt64(new Int64[] { 0,0, tt }));
           
            
            string st ="\\"+ conc(drr.ToArray());

            im2d.SetMetaData("0020|0032", st);

            im2d.SetMetaData("0020,0013", depth.ToString());

            string basePath = @"C:\Users\m3xpgag\source\repos\ConsoleApp5\ConsoleApp5\di";

            string filePath = ".dcm";
            string combinedPath = Path.Combine(basePath, (depth.ToString() + filePath));
           
            ifw.SetFileName(combinedPath);
            ifw.Execute(im2d);
        }

        static string conc(double[] cot)
        {
            StringBuilder sb = new StringBuilder();
            for(int i=0; i<cot.Length; i++) {
                if (i != 0)
                    sb.Append("\\");

                sb.Append(cot[i].ToString("0.0#"));
            }

            return sb.ToString();
        }

        static Image getnth2dsliceof3dimage(Image im3d,uint n) {
            //uint[] arr = im3d.GetSize().ToArray();
            Image im2d = new Image(im3d.GetWidth(),im3d.GetHeight(),PixelIDValueEnum.sitkFloat32);
            //Console.WriteLine(im2d.GetDimension());
            //\\im2d.
           // VectorUInt32 vui = new VectorUInt32(new uint[] { 511, 511, 69 });

            for (uint i = 0; i <  im3d.GetWidth(); i++)
            {
                for (uint j = 0; j < im3d.GetHeight(); j++)
                {
                    VectorUInt32 vui3 = new VectorUInt32(new uint[] { i, j, n });
                    VectorUInt32 vui2 = new VectorUInt32(new uint[] { i, j });

                    float re = im3d.GetPixelAsFloat(vui3);
                    im2d.SetPixelAsFloat(vui3, re);
                    
                }
            }


                    return im2d;
        }

        static string Getcurrentime()
        {
            return DateTime.Now.ToString("H%m%s");
        }

        static string Getcurrendate()
        {
            return (DateTime.Today.Year.ToString()) +
                (DateTime.Now.ToString("MM")) + (DateTime.Now.ToString("dd"));
        }
    }

}
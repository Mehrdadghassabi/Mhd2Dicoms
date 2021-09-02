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
            // watch to determine execution time
            var watch = new Stopwatch();
            watch.Start();

            // reading the file
            ImageFileReader imageFileReader = new ImageFileReader();
            /*
             your directory including mhd and raw file should be set here
             */
            imageFileReader.SetFileName("C:\\Users\\m3xpgag\\source\\repos\\" +
                "ConsoleApp5\\ConsoleApp5\\sample.mhd");
            imageFileReader.SetOutputPixelType(PixelIDValueEnum.sitkFloat32);
            imageFileReader.SetImageIO("MetaImageIO");
            Image image=imageFileReader.Execute();
            ImageFileWriter imageFileWriter = new ImageFileWriter();
            imageFileWriter.KeepOriginalImageUIDOn();

         /*   # Write the 3D image as a series
              # IMPORTANT: There are many DICOM tags that need to be updated when you modify
              # an original image. This is a delicate opration and requires
              # knowledge of the DICOM standard. This example only modifies some.
              # For a more complete list of tags that need to be modified see:
              # http://gdcm.sourceforge.net/wiki/index.php/Writing_DICOM
              # If it is critical for your work to generate valid DICOM files,
              # It is recommended to use David Clunie's Dicom3tools to validate
              # the files:
              # http://www.dclunie.com/dicom3tools.html
            */

            //  \\|//
            //  //|\\

            String modification_time = Getcurrentime();
            String modification_date= Getcurrendate();

            VectorDouble direction_Vec = image.GetDirection();
            double[] direction = direction_Vec.ToArray();

            /*
             these tags should be wrote
             # Copy some of the tags and add the relevant tags indicating the change.
             # For the series instance UID (0020|000e), each of the components is a number,
             # cannot start with zero, and separated by a '.' We create a unique series ID
             # using the date and time. Tags of interest:
               */
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

            /*
             write all 2D slice of mhd image
             */
            for(uint i = 0; i < image.GetDepth(); i++)
            {
                Write_slice(series_tag_values, image, i, imageFileWriter);
            }
            watch.Stop();

            Console.WriteLine($"Final Execution Time: {watch.ElapsedMilliseconds} ms");
            Console.ReadKey();
        }

        static void Write_slice(ArrayList series_tag_values, Image im3d,uint depth, ImageFileWriter ifw)
        {
            /*
            write one of the 3d image slices with given depth
             */
            var watch = new Stopwatch();
            watch.Start();
            Image im2d = getnth2dsliceof3dimage(im3d, depth);

            watch.Stop();

            Console.WriteLine($"getnth2dsliceof3dimage Execution Time: {watch.ElapsedMilliseconds} ms");
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

            /*
             your directory Saving Dicoms should be set here
             */
            string basePath = @"C:\Users\m3xpgag\source\repos\ConsoleApp5\ConsoleApp5\di";
            //the Dicom Suffix
            string filePath = ".dcm";
            string combinedPath = Path.Combine(basePath, (depth.ToString() + filePath));
           
            ifw.SetFileName(combinedPath);
            ifw.Execute(im2d);
        }

        static string conc(double[] cot)
        {
            /*
             simple method to Convert double to str
             then concat them
             */
            StringBuilder sb = new StringBuilder();
            for(int i=0; i<cot.Length; i++) {
                if (i != 0)
                    sb.Append("\\");

                sb.Append(cot[i].ToString("0.0#"));
            }

            return sb.ToString();
        }

        static Image getnth2dsliceof3dimage(Image im3d,uint n) {
            /**
             * take one of slice of 3d image with given depth
             * this method needs Complexity improvement
             * I get the 2d-image by Every pixel of given depth
             * then inserting all in a new 2d image
             * **/
            Image im2d = new Image(im3d.GetWidth(),im3d.GetHeight(),PixelIDValueEnum.sitkFloat32);
         

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
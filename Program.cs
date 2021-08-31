using System;
using System.Collections;
using System.Collections.Generic;
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
            ImageFileReader imageFileReader = new ImageFileReader();
            imageFileReader.SetFileName("C:\\Users\\m3xpgag\\source\\repos\\" +
                "ConsoleApp5\\ConsoleApp5\\sample.mhd");
            imageFileReader.SetOutputPixelType(PixelIDValueEnum.sitkFloat32);
            imageFileReader.SetImageIO("MetaImageIO");
            Image image=imageFileReader.Execute();


            //Image imo = new Image;
            

           // Console.WriteLine(imo);

         ImageFileWriter imageFileWriter = new ImageFileWriter();
            imageFileWriter.KeepOriginalImageUIDOn();
            //imageFileWriter.E
            //PixelIDValueEnum pe=new PixelIDValueEnum();
            //pe
            //RescaleIntensityImageFilter
          //  \\|//
          //  //|\\

            String modification_time=DateTime.Now.ToString("H%m%s");
            String modification_date=(DateTime.Today.Year.ToString())+
                (DateTime.Now.ToString("MM"))+(DateTime.Now.ToString("dd"));

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
            //Console.WriteLine(image.GetDimension());

           // VectorUInt32 imagesize = image.GetSize();
           // uint[] arr = imagesize.ToArray();
           // VectorUInt32 vui = new VectorUInt32(new uint[] { 511, 511,69 });
            
            //CREATE 2D SLICE TOMORROW
            
           // Console.WriteLine(image.GetPixelAsFloat(vui));

           /* foreach (uint i in arr)
                Console.WriteLine(i);*/

            //foreach (double d in ori)
            //Console.WriteLine(d)

            Console.WriteLine(getnth2dsliceof3dimage(image,3).GetPixelAsFloat(new VectorUInt32(new uint[] { 511, 511 })));
            Console.ReadLine();
        }

        static void Write_slice(ArrayList series_tag_values, Image im3d,string out_dir,int depth)
        {

        }

        static string conc(double[] direction)
        {
            StringBuilder sb = new StringBuilder();
            for(int i=0; i<direction.Length; i++) {
                if (i != 0)
                    sb.Append("\\");

                sb.Append(direction[i].ToString("0.0#"));
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
    }

}

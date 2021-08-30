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

            ImageFileWriter imageFileWriter = new ImageFileWriter();
            imageFileWriter.KeepOriginalImageUIDOn();

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

            VectorUInt32 imagesize = image.GetSize();
            uint[] arr = imagesize.ToArray();
            VectorUInt32 vui = new VectorUInt32(new uint[] { 511, 511,69 });
            Image im = new Image();
            //CREATE 2D SLICE TOMORROW
            Console.WriteLine(image.GetPixelAsFloat(vui));

            foreach (uint i in arr)
                Console.WriteLine(i);

            //foreach (double d in ori)
            //Console.WriteLine(d)

            Console.WriteLine(image);
            Console.ReadLine();
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

     
    }

}

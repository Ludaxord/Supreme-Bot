using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Net;
using System.IO;
using HtmlAgilityPack;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Xml.Linq;
using System.Drawing.Drawing2D;

namespace Supreme_Bot
{
    public class Supreme_Spy
    {
        public Supreme_Spy()
        {
        }

        public string mainUrl = "https://supremenewyork.com";

        public bool foundedObject = false;

        public bool browserOpened = false;

        public string chooseSize = null;

        public Dictionary<string, int> possibleUrl = new Dictionary<string, int>();

        string GetHtml(string url)
        {
            string Shtml = string.Empty;
            try
            {

                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);

                webRequest.Method = "GET";

                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();

                StreamReader webSource = new StreamReader(webResponse.GetResponseStream());


                Shtml = webSource.ReadToEnd();
                webResponse.Close();
            }
            catch
            {
                Console.WriteLine(" error: " + url + Environment.NewLine);
            }

            return Shtml;
        }

        public void GetSupremeItem()
        {
            string source = GetHtml("https://www.supremenewyork.com/shop/new");
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(source);
            HtmlNodeCollection contentCollection;
            contentCollection = doc.DocumentNode.SelectNodes(".//div[@class='inner-article']");
            var imageCollection = doc.DocumentNode.SelectNodes(".//div[@class='inner-article']/a[@href]");
            var urls = doc.DocumentNode.Descendants("img")
                                .Select(e => e.GetAttributeValue("src", null))
                                .Where(s => !String.IsNullOrEmpty(s));

            int index = 0;
            Console.WriteLine("Podaj nazwę pliku do porównania:" + Environment.NewLine);
            string name = Console.ReadLine();
            Console.WriteLine("Podaj rozmiar: Small, Medium, Large, XLarge, Uni");
            chooseSize = Console.ReadLine();
            List<int> list = new List<int>();
            string testImagePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/" + name + ".jpg";
            Console.WriteLine(testImagePath);
            Bitmap TestImage = new Bitmap(testImagePath);
            var isRightSize = CheckSize(TestImage);
            var size = false;

            if (isRightSize == true) {
                size = true;
            } else {
                TestImage = ResizeImage(450, 450, TestImage);
                size = true;
            }

            if (size == true) {
                foreach (var url in urls)
                {
                    var bigImageUrl = ReplaceURLForLargeImage(url);
                    SaveImage(bigImageUrl, index, name, list, doc, TestImage);
                    index++;

                    if (foundedObject == true)
                    {
                        break;
                    }
                }
            }

            //Max element of list

            var maxValue = list.Max();

            Console.WriteLine("highest value from list: " + maxValue);

            Console.WriteLine("Comparison for: " + name);

            if (foundedObject == false)
            {
                foreach (KeyValuePair<string, int> foundedUrl in possibleUrl)
                {
                    if (foundedUrl.Value == maxValue)
                    {
                        AddToBasket(foundedUrl.Key);
                        foundedObject = true;
                    }
                }

            }

        }

        public string ReplaceURLForLargeImage(string url)
        {
            StringBuilder builder = new StringBuilder(url);
            builder.Replace("/vi/", "/ma/");
            string y = builder.ToString();
            return "https:" + y;
        }

        public static bool ImageCompareString(Bitmap firstImage, Bitmap secondImage)
        {
            MemoryStream ms = new MemoryStream();
            firstImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            String firstBitmap = Convert.ToBase64String(ms.ToArray());
            ms.Position = 0;

            secondImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            String secondBitmap = Convert.ToBase64String(ms.ToArray());

            if (firstBitmap.Equals(secondBitmap))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool ImageCompareArray(Bitmap firstImage, Bitmap secondImage)
        {
            bool flag = true;
            string firstPixel;
            string secondPixel;

            if (firstImage.Width == secondImage.Width && firstImage.Height == secondImage.Height)
            {
                for (int i = 0; i < firstImage.Width; i++)
                {
                    for (int j = 0; j < firstImage.Height; j++)
                    {
                        firstPixel = firstImage.GetPixel(i, j).ToString();
                        secondPixel = secondImage.GetPixel(i, j).ToString();
                        if (firstPixel != secondPixel)
                        {
                            flag = false;
                            break;
                        }
                    }
                }

                if (flag == false)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        private static bool Equals(Bitmap bmp1, Bitmap bmp2)
        {
            if (!bmp1.Size.Equals(bmp2.Size))
            {
                return false;
            }
            for (int x = 0; x < bmp1.Width; ++x)
            {
                for (int y = 0; y < bmp1.Height; ++y)
                {
                    if (bmp1.GetPixel(x, y) != bmp2.GetPixel(x, y))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public void SaveImage(string url, int index, string testFileName, List<int> list, HtmlDocument doc, Bitmap resizedImage)
        {
            var name = url.Replace("https://assets.supremenewyork.com/", "");
            name = name.Substring(name.IndexOf('/') + 1);
            name = name.Replace("ma/", "");
            Console.WriteLine(url);

            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            Stream responseStream = response.GetResponseStream();

            Bitmap supremeImage = new Bitmap(responseStream);

            bool compareByArray = ImageCompareArray(supremeImage, resizedImage);
            bool compareByBase64 = ImageCompareString(supremeImage, resizedImage);
            bool isEqual = TestDifferenceInImages(resizedImage, supremeImage);
            bool compare = Equals(resizedImage, supremeImage);
            bool algorythmCompare = AreEqual(resizedImage, supremeImage);
            List<bool> iHash1 = GetHash(supremeImage);
            List<bool> iHash2 = GetHash(resizedImage);

            //determine the number of equal pixel (x of 256)
            int equalElements = iHash1.Zip(iHash2, (i, j) => i == j).Count(eq => eq);

            list.Add(equalElements);

            if (equalElements >= 200)
            {
                Console.WriteLine("more than 200: " + equalElements);
                Console.WriteLine(url);
            }

            Console.WriteLine("Compare by array: " + compareByArray);
            Console.WriteLine("Compare by string: " + compareByBase64);
            Console.WriteLine("Compare by size: " + isEqual);
            Console.WriteLine("Compare with Equals function: " + compare);
            Console.WriteLine("Compare by pixel in photo: " + equalElements);

            if (compareByArray || compareByBase64 || isEqual || compare)
            {
                var u = url.Replace("https:", "");
                u = u.Replace("/ma/", "/vi/");
                var divs = doc.DocumentNode.SelectNodes(".//div[@class='inner-article']");
                if (divs != null)
                {
                    foreach (var div in divs)
                    {
                        if (div.InnerHtml.Contains(u))
                        {
                            var d = div.Descendants("a").Select(e => e.GetAttributeValue("href", null));
                            foreach (var s in d)
                            {
                                var productUrl = $"{mainUrl}{s}";
                                Console.WriteLine(productUrl);
                                AddToBasket(productUrl);
                                foundedObject = true;
                                //OpenWebBrowser(productUrl);
                                break;
                            }
                        }
                    }
                }
            }

            if (equalElements >= 230)
            {
                var u = url.Replace("https:", "");
                u = u.Replace("/ma/", "/vi/");
                var divs = doc.DocumentNode.SelectNodes(".//div[@class='inner-article']");
                if (divs != null)
                {
                    foreach (var div in divs)
                    {
                        if (div.InnerHtml.Contains(u))
                        {
                            var d = div.Descendants("a").Select(e => e.GetAttributeValue("href", null));
                            foreach (var s in d)
                            {
                                var productUrl = $"{mainUrl}{s}";
                                Console.WriteLine(productUrl);
                                possibleUrl.Add(productUrl, equalElements);
                                break;
                            }
                        }
                    }
                }
            }
        }

        bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        public void SimulateBasket()
        {
        }

        public void AddToBasket(string productUrl)
        {
            string source = GetHtml(productUrl);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(source);
            HtmlNodeCollection forms = doc.DocumentNode.SelectNodes(".//div[@id='cctrl']");
            foreach (var form in forms)
            {
                string formattedForm;
                try
                {
                    formattedForm = XElement.Parse(form.InnerHtml).ToString();

                }
                catch
                {
                    formattedForm = form.InnerHtml;
                }
                //Console.WriteLine(formattedForm);
                var options = form.Descendants("option");
                var productsStyle = form.Descendants("input").Select(e => e.GetAttributeValue("value", null));
                //var optionValue = form.Descendants("option").Select(e => e.GetAttributeValue("value", null));
                var formAction = form.Descendants("form").Select(e => e.GetAttributeValue("action", null));
                var formMethod = form.Descendants("form").Select(e => e.GetAttributeValue("method", null));
                var actionUrl = mainUrl;
                var productStyleCode = "";
                var productSizeId = "";
                var productSize = "";
                var productMethod = "";
                foreach (var productStyle in productsStyle)
                {
                    if (IsDigitsOnly(productStyle))
                    {
                        productStyleCode = productStyle;
                    }
                }
                foreach (var option in options)
                {
                    var size = option.InnerText;
                    if (size == chooseSize)
                    {
                        productSize = size;
                        var values = option.SelectNodes($"//option[text()='{chooseSize}']").Select(a => a.GetAttributeValue("value", null));
                        foreach (var sizeId in values)
                        {
                            productSizeId = sizeId;
                        }
                    } else if (chooseSize == "Uni") {

                    }
                }
                foreach (var action in formAction)
                {
                    actionUrl += action;
                }
                foreach (var method in formMethod)
                {
                    productMethod = method;
                }
                Console.WriteLine("////////////////////////////////////////////////////////////////////");
                Console.WriteLine("product style code: " + productStyleCode);
                Console.WriteLine("product size id code: " + productSizeId);
                Console.WriteLine("product size: " + productSize);
                Console.WriteLine("product action Url: " + actionUrl);
                Console.WriteLine("product method: " + productMethod);

                var chrome = new ChromeActions(chooseSize);

                chrome.InitChrome(productUrl);
            }
        }

        /*
         * Check hash of image
         */

        public static List<bool> GetHash(Bitmap bmpSource)
        {
            List<bool> lResult = new List<bool>();
            //create new image with 16x16 pixel
            Bitmap bmpMin = new Bitmap(bmpSource, new Size(16, 16));
            for (int j = 0; j < bmpMin.Height; j++)
            {
                for (int i = 0; i < bmpMin.Width; i++)
                {
                    //reduce colors to true / false                
                    lResult.Add(bmpMin.GetPixel(i, j).GetBrightness() < 0.5f);
                }
            }
            return lResult;
        }

        /*
         * Check if image sizes are same 
         */

        public static bool TestDifferenceInImages(Bitmap bmp1, Bitmap bmp2)
        {
            bool equals = true;
            Rectangle rectBmp1 = new Rectangle(0, 0, bmp1.Width, bmp1.Height);
            Rectangle rectBmp2 = new Rectangle(0, 0, bmp2.Width, bmp2.Height);
            BitmapData bmpData1 = bmp1.LockBits(rectBmp1, ImageLockMode.ReadOnly, bmp1.PixelFormat);
            BitmapData bmpData2 = bmp2.LockBits(rectBmp2, ImageLockMode.ReadOnly, bmp2.PixelFormat);
            unsafe
            {
                byte* ptr1 = (byte*)bmpData1.Scan0.ToPointer();
                byte* ptr2 = (byte*)bmpData2.Scan0.ToPointer();
                int width = rectBmp1.Width * 3;
                for (int y = 0; equals && y < rectBmp1.Height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (*ptr1 != *ptr2)
                        {
                            equals = false;
                            break;
                        }
                        ptr1++;
                        ptr2++;
                    }
                    ptr1 += bmpData1.Stride - width;
                    ptr2 += bmpData2.Stride - width;
                }
            }
            bmp1.UnlockBits(bmpData1);
            bmp2.UnlockBits(bmpData2);

            return equals;
        }

        /*
            * Check compare in unsafe mode
        */

        public unsafe static bool AreEqual(Bitmap b1, Bitmap b2)
        {
            if (b1.Size != b2.Size)
            {
                return false;
            }

            if (b1.PixelFormat != b2.PixelFormat)
            {
                return false;
            }

            if (b1.PixelFormat != PixelFormat.Format32bppArgb)
            {
                return false;
            }

            Rectangle rect = new Rectangle(0, 0, b1.Width, b1.Height);
            BitmapData data1
                = b1.LockBits(rect, ImageLockMode.ReadOnly, b1.PixelFormat);
            BitmapData data2
                = b2.LockBits(rect, ImageLockMode.ReadOnly, b1.PixelFormat);

            int* p1 = (int*)data1.Scan0;
            int* p2 = (int*)data2.Scan0;
            int byteCount = b1.Height * data1.Stride / 4; //only Format32bppArgb 

            bool result = true;
            for (int i = 0; i < byteCount; ++i)
            {
                if (*p1++ != *p2++)
                {
                    result = false;
                    break;
                }
            }

            b1.UnlockBits(data1);
            b2.UnlockBits(data2);

            return result;
        }

        public Bitmap ResizeImage(int newWidth, int newHeight, Bitmap srcImage) {
            Bitmap newImage = new Bitmap(newWidth, newHeight);

            using (Graphics gr = Graphics.FromImage(newImage))
            {
                gr.SmoothingMode = SmoothingMode.HighQuality;
                gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                gr.DrawImage(srcImage, new Rectangle(0, 0, newWidth, newHeight));
            }

            return newImage;
        }

        public bool CheckSize(Bitmap bitmap) {
            var width = bitmap.Width;
            var height = bitmap.Height;
            Console.WriteLine($"width of source: {width}, height of source: {height}");
            return width == 450 && height == 450 ? true : false;
        }

        /*
        deprecated
        */

        public void ShowNewItemsPage()
        {
            var client = new WebClient();
            var contents = client.DownloadString("https://www.supremenewyork.com/shop/new");
            Console.WriteLine(contents);
        }
    }
}

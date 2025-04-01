using System.Text;

namespace DoAnNhom6.Helpers
{
    public class MyUntil
    {
        public static string UploadHinh(IFormFile Hinh,string folder)
        {
            //try
            //{
            //    var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Hinh/Hinh", folder, Hinh.FileName);
            //    using (var myfile = new FileStream(fullPath, FileMode.CreateNew))
            //    {
            //        Hinh.CopyTo(myfile);
            //    }
            //    return Hinh.FileName;
            //}
            //catch(Exception ex) {
            //    return string.Empty;
            //}
            try
            {
                // Get the path where you want to save the image
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Hinh/Hinh", folder);

                // Create the directory if it doesn't exist
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // Generate a unique filename (you can use other methods if needed)
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(Hinh.FileName);
                var fullPath = Path.Combine(uploadPath, uniqueFileName);

                // Save the image to the specified path
                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    Hinh.CopyTo(fileStream);
                }

                return uniqueFileName;
            }
            catch (Exception ex)
            {
                // Handle any exceptions (e.g., log or return an error message)
                return string.Empty;
            }
        }
    }
}

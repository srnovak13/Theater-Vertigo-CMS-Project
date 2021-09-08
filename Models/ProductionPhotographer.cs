using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheatreCMS3.Models;

namespace TheatreCMS3.Areas.Prod.Models
{
    public class ProductionPhotographer : ApplicationUser
    {
        public string Camera { get; set; }
        public double CameraCost { get; set; }
        public string CameraSerialNumber { get; set; }

        public static void SeedProductionPhotographers(UserManager<ApplicationUser> userManager)
        {
            // Seeds a default ProductionPhotographer user
            if (userManager.FindByNameAsync("ProductionPhotographer").Result == null)
            {
                var user = new ProductionPhotographer
                {
                    Id = "2",
                    UserName = "ProductionPhotographer",
                    Email = "photos@theatrevertigo.com",
                    Camera = "Camera",
                    CameraCost = 500.00,
                    CameraSerialNumber = "camera serial"
                };

                // This is where the password is set
                IdentityResult result = userManager.CreateAsync(user, "photos").Result;

                if (result.Succeeded)
                    userManager.AddToRoleAsync(user.Id, "Production Photographer").Wait();
            }
        }
    }
}
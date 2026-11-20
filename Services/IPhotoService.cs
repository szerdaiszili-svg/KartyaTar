using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KartyaTar2.Services
{
    public interface IPhotoService
    {
        Task<string?> TakePhotoAsync();
        Task<string?> PickPhotoAsync();
        void DeletePhoto(string? path);
    }
}

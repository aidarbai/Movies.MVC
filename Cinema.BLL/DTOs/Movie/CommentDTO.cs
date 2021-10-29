using Cinema.BLL.DTOs.User;
using Cinema.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.BLL.DTOs.Movie
{
    public class CommentDTO
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int MovieId { get; set; }
        public  ApplicationUserDTO User { get; set; }
    }
}

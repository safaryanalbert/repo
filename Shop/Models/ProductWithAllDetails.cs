using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Shop.Models
{
    public class ProductWithAllDetails
    {
        [Key]
        public int Id { get; set; }

        [Key]
        public int ProductTypeId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "* Required")]
        [MinLength(2, ErrorMessage = "Incorect Input:Your input must be long from two characters")]
        [RegularExpression(@"^[a-zA-Z]+[ a-zA-Z-_]*$", ErrorMessage = "Incorrect Input:Please enter only letters")]

        public string Name { get; set;}

        public string About { get; set;}

        public string Path { get; set;}

        [Required(AllowEmptyStrings = false, ErrorMessage = "* Required")]
        //[RegularExpression(@"^[a-zA-Z]+[ a-zA-Z-_]*$", ErrorMessage = "Incorrect Input:Please enter only letters")]
        public List<string>Types { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "* Required")]
        [RegularExpression(@"(^\d+$)", ErrorMessage = "Your input must be only digits and big from 0")]
        public List<int ?> Prices { get; set; }
    }
}
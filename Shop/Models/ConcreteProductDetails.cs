//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Shop.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class ConcreteProductDetails
    {
        [Key]
        public int ConcreteProductDetails_Id { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "* Required")]
        public string ConcreteProductTypeName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "* Required")]
        [RegularExpression(@"(^\d+$)", ErrorMessage = "Your input must be only digits and big from 0")]
        public Nullable<int> Price { get;set;}
        [Key]
        public int ConcreteProduct_Id { get; set; }    
        public virtual ConcreteProduct ConcreteProduct { get; set; }
    }
}
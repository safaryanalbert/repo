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

    public partial class ProductType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProductType()
        {
            this.ConcreteProduct = new HashSet<ConcreteProduct>();
        }
        [Key]
        public int ProductType_Id { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "* Required")]
        [MinLength(2, ErrorMessage = "Incorect ProductTypeName:Your input must be long from two characters")]
        [RegularExpression(@"^[a-zA-Z]+[ a-zA-Z-_]*$", ErrorMessage = "Incorrect ProductTypeName:Please enter only letters")]
        public string ProductTypeName { get; set; }
        [Key]
        public int Product_Id { get; set; }  
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ConcreteProduct> ConcreteProduct { get; set; }
        public virtual Produts Produts { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Database.Table
{
    public class Category
    {
        [Key]
        public Guid Id { get; set; }

        // Tiều đề Category
        [Required(ErrorMessage = "Phải có tên danh mục")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "{0} dài {1} đến {2}")]
        [Display(Name = "Tên danh mục")]
        public string Title { get; set; }

        // Nội dung, thông tin chi tiết về Category
        [DataType(DataType.Text)]
        [Display(Name = "Nội dung danh mục")]
        public string Content { set; get; }

        //chuỗi Url
        [Required(ErrorMessage = "Phải tạo url")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "{0} dài {1} đến {2}")]
        [RegularExpression(@"^[a-z0-9-]*$", ErrorMessage = "Chỉ dùng các ký tự [a-z0-9-]")]
        [Display(Name = "Url hiện thị")]
        public string Slug { set; get; }

        public bool IsDeleted { get; set; } = false;
        // Các Category con
        public ICollection<Category>? CategoryChildren { get; set; }

        // Category cha (FKey)
        [Display(Name = "Danh mục cha")]
        public Guid? ParentCategoryId { get; set; }

        [ForeignKey("ParentCategoryId")]
        [Display(Name = "Danh mục cha")]
        public Category? ParentCategory { set; get; }
    }
}

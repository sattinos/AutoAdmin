﻿using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace AutoAdmin.Model {
    /// <summary>
    /// T will be based on business needs. Example values are: byte, ushort, uint, ulong, Guid, etc....
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseEntity<T> {
        public BaseEntity(bool isIdAutoGenerated = false)
        {
            IsIdAutoGenerated = isIdAutoGenerated;
        }
        public T Id { get; set; }
        [DataType(DataType.Date)]
        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }        
        public string UpdatedBy { get; set; }

        public bool IsIdAutoGenerated { get; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}

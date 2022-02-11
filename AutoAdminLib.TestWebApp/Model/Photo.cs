using System;
using AutoAdminLib.Model;

namespace AutoAdminLib.TestWebApp.Model {
    public class Photo : BaseEntity<Guid> {
        public string Name { get; set; }
        public Guid FileId { get; set; }
    }
}

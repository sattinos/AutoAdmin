using System;

namespace AutoAdmin.Model {
    public class Photo : BaseEntity<Guid> {
        public string Name { get; set; }
        public Guid FileId { get; set; }
    }
}

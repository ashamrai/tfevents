//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TFSServicesDBLib
{
    using System;
    using System.Collections.Generic;
    
    public partial class Rules
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Rules()
        {
            this.IsDeleted = false;
            this.RunHistory = new HashSet<RunHistory>();
            this.RulesRevisions = new HashSet<Revisions>();
        }
    
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string TriggerScript { get; set; }
        public string ProcessScript { get; set; }
        public int RuleTypeId { get; set; }
        public int Revision { get; set; }
        public bool IsDeleted { get; set; }
    
        public virtual RuleType RuleType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RunHistory> RunHistory { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Revisions> RulesRevisions { get; set; }
    }
}

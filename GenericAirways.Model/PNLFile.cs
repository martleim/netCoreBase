//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GenericAirways.Model
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.ComponentModel.DataAnnotations;
    
    [DataContract]
    public partial class PNLFile
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PNLFile()
        {
            this.RecordLocator = new HashSet<RecordLocator>();
        }
    
        
    	[Key()]
    	[DataMember(Name = "Id")]
    	public int Id { get; set; }
        
    	[DataMember(Name = "File")]
    	public byte[] File { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [DataMember(Name = "RecordLocator")]
    	public virtual ICollection<RecordLocator> RecordLocator { get; set; }
    }
}

﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BookInventorySystem.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("BookInventorySystem.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to dbo.Customer_Insert @CustomerId, @CustomerName, @Address,@PhoneNo.
        /// </summary>
        internal static string AddUser {
            get {
                return ResourceManager.GetString("AddUser", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to BookUpdate.
        /// </summary>
        internal static string BookUpdate {
            get {
                return ResourceManager.GetString("BookUpdate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to dbo.CheckIn_Book @BookId, @CustomerId, @Quantity.
        /// </summary>
        internal static string CheckInBook {
            get {
                return ResourceManager.GetString("CheckInBook", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to dbo.CheckOut_Book @OrderId, @BookId, @CustomerId, @DateTime,@HasBook, @Quantity.
        /// </summary>
        internal static string CheckOutBook {
            get {
                return ResourceManager.GetString("CheckOutBook", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CustomerUpdate.
        /// </summary>
        internal static string CustomerUpdate {
            get {
                return ResourceManager.GetString("CustomerUpdate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to dbo.Book_Delete @BookId, @BookName, @Quantity, @AuthorName.
        /// </summary>
        internal static string DeleteBook {
            get {
                return ResourceManager.GetString("DeleteBook", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to dbo.Customer_Delete @CustomerId, @CustomerName, @Address, @PhoneNo.
        /// </summary>
        internal static string DeleteCustomer {
            get {
                return ResourceManager.GetString("DeleteCustomer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to dbo.Get_AllBook.
        /// </summary>
        internal static string GetAllBooks {
            get {
                return ResourceManager.GetString("GetAllBooks", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to dbo.GetAllBookBelongToACustomer @CustomerId.
        /// </summary>
        internal static string GetAllBooksOfSelectedCustomer {
            get {
                return ResourceManager.GetString("GetAllBooksOfSelectedCustomer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to dbo.Get_AllCustomers.
        /// </summary>
        internal static string GetAllCustomer {
            get {
                return ResourceManager.GetString("GetAllCustomer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to dbo.Get_AllCustomersWhoHasBook.
        /// </summary>
        internal static string GetAllCustomerHavingBook {
            get {
                return ResourceManager.GetString("GetAllCustomerHavingBook", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to dbo.Get_AllOrders.
        /// </summary>
        internal static string GetAllOrders {
            get {
                return ResourceManager.GetString("GetAllOrders", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to dbo.Book_Insert @BookId, @BookName, @Quantity, @AuthorName.
        /// </summary>
        internal static string InsertBook {
            get {
                return ResourceManager.GetString("InsertBook", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to dbo.Book_Update @BookId, @BookName, @Quantity, @AuthorName.
        /// </summary>
        internal static string UpdateBook {
            get {
                return ResourceManager.GetString("UpdateBook", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to dbo.Customer_Update @CustomerId, @CustomerName, @Address, @PhoneNo.
        /// </summary>
        internal static string UpdateCustomer {
            get {
                return ResourceManager.GetString("UpdateCustomer", resourceCulture);
            }
        }
    }
}

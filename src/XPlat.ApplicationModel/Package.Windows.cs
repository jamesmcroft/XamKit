// XPlat Apps licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if WINDOWS_UWP
namespace XPlat.ApplicationModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using XPlat.Storage;

    /// <summary>Provides information about a package.</summary>
    public class Package : IPackage, IPackage2, IPackage3
    {
        private static Package current;

        private readonly WeakReference originatorReference;

        private List<IPackage> dependencies;

        private IPackageId id;

        /// <summary>
        /// Initializes a new instance of the <see cref="Package"/> class with the Windows application package.
        /// </summary>
        /// <param name="package">The Windows application package.</param>
        /// <exception cref="T:System.ArgumentNullException">Thrown if <paramref name="package"/> is <see langword="null"/>.</exception>
        public Package(Windows.ApplicationModel.Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            this.originatorReference = new WeakReference(package);
        }

        /// <summary>Gets the package for the current app.</summary>
        public static Package Current => current != null && current.originatorReference.IsAlive ? current : (current = Windows.ApplicationModel.Package.Current);

        /// <summary>Gets the package identity of the current package.</summary>
        public IPackageId Id => this.id ??= new PackageId(this.Originator.Id);

        /// <summary>Gets the location of the installed package.</summary>
        public IStorageFolder InstalledLocation => new StorageFolder(this.Originator.InstalledLocation);

        /// <summary>Gets the packages on which the current package depends.</summary>
        public IReadOnlyList<IPackage> Dependencies =>
            this.dependencies ??= this.Originator.Dependencies
                .Select(item => new Package(item))
                .Cast<IPackage>()
                .ToList();

        /// <summary>Gets the display name of the package.</summary>
        public string DisplayName => this.Originator.DisplayName;

        /// <summary>Gets the logo of the package.</summary>
        public Uri Logo => this.Originator.Logo;

        /// <summary>Indicates whether the package is installed in development mode.</summary>
        public bool IsDevelopmentMode => this.Originator.IsDevelopmentMode;

        /// <summary>Gets the date on which the application package was installed or last updated.</summary>
        public DateTimeOffset InstalledDate => this.Originator.InstalledDate;

        /// <summary>Gets the original Windows Package reference object.</summary>
        public Windows.ApplicationModel.Package Originator =>
            this.originatorReference != null && this.originatorReference.IsAlive
                ? this.originatorReference.Target as Windows.ApplicationModel.Package
                : null;

        /// <summary>
        /// Allows conversion of a <see cref="Windows.ApplicationModel.Package"/> to the <see cref="Package"/> without direct casting.
        /// </summary>
        /// <param name="package">
        /// The <see cref="Windows.ApplicationModel.Package"/>.
        /// </param>
        /// <returns>
        /// The <see cref="Package"/>.
        /// </returns>
        public static implicit operator Package(Windows.ApplicationModel.Package package)
        {
            return new Package(package);
        }

        /// <summary>
        /// Allows conversion of a <see cref="Package"/> to the <see cref="Windows.ApplicationModel.Package"/> without direct casting.
        /// </summary>
        /// <param name="package">
        /// The <see cref="Package"/>.
        /// </param>
        /// <returns>
        /// The <see cref="Windows.ApplicationModel.Package"/>.
        /// </returns>
        public static implicit operator Windows.ApplicationModel.Package(Package package)
        {
            return package.Originator;
        }
    }
}
#endif
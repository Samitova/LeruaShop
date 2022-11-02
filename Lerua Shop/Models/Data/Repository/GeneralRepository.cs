using Lerua_Shop.Models.Data.EF;
using Lerua_Shop.Models.ModelsDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lerua_Shop.Models.Data.Repository
{
    public class GeneralRepository : IDisposable
    {
        // pattern singelton and unit of work
        private static GeneralRepository _instance;

        private bool _disposed = false;
        private DbStoreContext _context = new DbStoreContext();

        private BaseRepository<PageDTO> _pagesRepository;
        private BaseRepository<SidebarDTO> _sidebarsRepository;
        private BaseRepository<CategoryDTO> _categoriesRepository;
        private BaseRepository<ProductDTO> _productsRepository;
        private BaseRepository<UserDTO> _usersRepository;
        private BaseRepository<RoleDTO> _rolesRepository;
        private BaseRepository<UserRoleDTO> _userRolesRepository;

        public BaseRepository<PageDTO> PagesRepository
        {
            get
            {

                if (this._pagesRepository == null)
                {
                    this._pagesRepository = new BaseRepository<PageDTO>(_context);
                }
                return _pagesRepository;
            }
        }

        public BaseRepository<SidebarDTO> SidebarsRepository
        {
            get
            {

                if (this._sidebarsRepository == null)
                {
                    this._sidebarsRepository = new BaseRepository<SidebarDTO>(_context);
                }
                return _sidebarsRepository;
            }
        }

        public BaseRepository<CategoryDTO> CategoriesRepository
        {
            get
            {

                if (this._categoriesRepository == null)
                {
                    this._categoriesRepository = new BaseRepository<CategoryDTO>(_context);
                }
                return _categoriesRepository;
            }
        }

        public BaseRepository<ProductDTO> ProductsRepository
        {
            get
            {

                if (this._productsRepository == null)
                {
                    this._productsRepository = new BaseRepository<ProductDTO>(_context);
                }
                return _productsRepository;
            }
        }

        public BaseRepository<UserDTO> UsersRepository
        {
            get
            {

                if (this._usersRepository == null)
                {
                    this._usersRepository = new BaseRepository<UserDTO>(_context);
                }
                return _usersRepository;
            }
        }

        public BaseRepository<RoleDTO> RolesRepository
        {
            get
            {

                if (this._rolesRepository == null)
                {
                    this._rolesRepository = new BaseRepository<RoleDTO>(_context);
                }
                return _rolesRepository;
            }
        }

        public BaseRepository<UserRoleDTO> UserRolesRepository
        {
            get
            {

                if (this._userRolesRepository == null)
                {
                    this._userRolesRepository = new BaseRepository<UserRoleDTO>(_context);
                }
                return _userRolesRepository;
            }
        }

        private GeneralRepository()
        { }

        public static GeneralRepository GetInstance()
        {
            if (_instance == null)
                _instance = new GeneralRepository();
            return _instance;
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this._disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
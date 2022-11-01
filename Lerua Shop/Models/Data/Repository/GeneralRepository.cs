﻿using Lerua_Shop.Models.Data.EF;
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
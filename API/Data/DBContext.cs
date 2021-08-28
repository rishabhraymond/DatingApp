﻿using API.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data {
    public class DBContext: DbContext {
        public DBContext(DbContextOptions options): base(options) {

        }

        public DbSet<User> Users { get; set; }
    }
}

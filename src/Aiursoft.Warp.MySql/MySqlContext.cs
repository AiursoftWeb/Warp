using System.Diagnostics.CodeAnalysis;
using Aiursoft.Warp.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Warp.MySql;

[ExcludeFromCodeCoverage]

public class MySqlContext(DbContextOptions<MySqlContext> options) : WarpDbContext(options);

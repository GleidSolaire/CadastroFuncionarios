using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GerenciamentoDeFuncionarios.Models;

namespace GerenciamentoDeFuncionarios.Controllers
{
    public class DepartamentosController : Controller
    {
        private readonly FuncionariosContext _context;

        public DepartamentosController(FuncionariosContext context)
        {
            _context = context;
        }

        // GET: Departamentos
        public async Task<IActionResult> Index()
        {
            var funcionariosContext = _context.Departamento.Include(d => d.Responsavel);
            return View(await funcionariosContext.ToListAsync());
        }

        // GET: Departamentos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var departamento = await _context.Departamento
                .Include(d => d.Responsavel)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (departamento == null)
            {
                return NotFound();
            }

            return View(departamento);
        }

        // GET: Departamentos/Create
        public IActionResult Create()
        {
            ViewData["ResponsavelId"] = 
                new SelectList(
                    _context.Funcionario.Where(f => f.Lotacao == null)
                    , "Id", 
                    "Nome");
            return View();
        }

        // POST: Departamentos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,ResponsavelId")] Departamento departamento)
        {
            departamento.Responsavel = _context.Funcionario
                    .Find(departamento.ResponsavelId);
            ModelState.Clear();
            TryValidateModel(departamento);
            if (ModelState.IsValid)
            {
                if(departamento.Responsavel.Lotacao == null)
                {
                    departamento.Funcionarios.Add(departamento.Responsavel);
                }
                _context.Add(departamento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ResponsavelId"] = new SelectList(_context.Funcionario, "Id", "Nome", departamento.ResponsavelId);
            return View(departamento);
        }

        // GET: Departamentos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var departamento = await _context.Departamento.SingleOrDefaultAsync(m => m.Id == id);
            if (departamento == null)
            {
                return NotFound();
            }
            ViewData["ResponsavelId"] = 
                new SelectList(_context.Funcionario, "Id", "Nome", 
                departamento.ResponsavelId);
            return View(departamento);
        }

        // POST: Departamentos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,ResponsavelId")] Departamento departamento)
        {
            departamento.Responsavel = _context.Funcionario
                    .Find(departamento.ResponsavelId);
            ModelState.Clear();
            TryValidateModel(departamento);
            if (id != departamento.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(departamento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartamentoExists(departamento.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ResponsavelId"] = new SelectList(_context.Funcionario, "Id", "Nome", departamento.ResponsavelId);
            return View(departamento);
        }

        // GET: Departamentos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var departamento = await _context.Departamento
                .Include(d => d.Responsavel)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (departamento == null)
            {
                return NotFound();
            }

            return View(departamento);
        }

        // POST: Departamentos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var departamento = await _context.Departamento.SingleOrDefaultAsync(m => m.Id == id);
            _context.Departamento.Remove(departamento);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DepartamentoExists(int id)
        {
            return _context.Departamento.Any(e => e.Id == id);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebAppHotel.Data;
using WebAppHotel.Models;
using WebAppHotel.Models.Entities;
using WebAppHotel.Models.Enums;

namespace WebAppHotel.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly WebAppHotelContext _context;

        public ReservationsController(WebAppHotelContext context)
        {
            _context = context;
        }

        private bool isReserveDatesValid(Reservation reservation)
        {
            // The clostest check-out date has to be before the check-in date specified in the reservation.
            return (from reserve in _context.Reservation
                    where reserve.RoomId == reservation.RoomId
                    where reserve.CheckOut > reservation.CheckIn
                    where reserve.CheckOut < reservation.CheckOut
                    select reserve).IsNullOrEmpty();
        }


        // GET: Reservations
        public async Task<IActionResult> Index(int? id)
        {
            ViewData["Rooms"] = (from room in _context.Room
                                    select room).ToList();
            if (id == null)
            {
                ViewData["Customers"] = _context.Customer.ToList();
                return _context.Reservation != null ?
                            View(await _context.Reservation.ToListAsync()) :
                            Problem("Entity set 'WebAppHotelContext.Reservation'  is null.");
            }
            else
            {
                ViewData["Customers"] = (from customer in _context.Customer
                                         where customer.Id == id
                                         select customer).ToList();
                return _context.Reservation != null ?
                            View(await _context.Reservation.Where(r => r.CustomerId == id).ToListAsync()) :
                            Problem("Entity set 'WebAppHotelContext.Reservation'  is null.");
            }
        }

        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Reservation == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // GET: Reservations/Create
        public IActionResult Create(Customer customer)
        {
            ViewData["Rooms"] = _context.Room.ToArray();
            ViewData["statusOptions"] = Enum.GetNames(typeof(ReservationStatus));
            ViewData["CustomerId"] = customer.Id;
            ViewData["CustomerName"] = customer.Name;
            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RoomId,CustomerId,CheckIn,CheckOut,Price,Status")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                if (isReserveDatesValid(reservation))
                {
                    throw new ArgumentException($"Room is not available at the selected time.");
                }

                StatusTimelineLog checkInLog = new(reservation.CheckIn, RoomStatus.Unavailable, reservation.RoomId);
                StatusTimelineLog checkOutLog = new(reservation.CheckOut, RoomStatus.Available, reservation.RoomId);

                _context.statusTimelineLog.Add(checkInLog);
                _context.statusTimelineLog.Add(checkOutLog);

                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction("Index", "Customers");
        }

        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Reservation == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            ViewData["CustomerName"] = (from customer in _context.Customer
                                       where customer.Id == reservation.CustomerId
                                       select customer.Name).Single();
            ViewData["Rooms"] = _context.Room.ToArray();
            ViewData["statusOptions"] = Enum.GetNames(typeof(ReservationStatus));
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RoomId,CustomerId,CheckIn,CheckOut,Price,Status")] Reservation reservation)
        {
            if (id != reservation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (isReserveDatesValid(reservation))
                    {
                        throw new ArgumentException($"Room is not available at the selected time.");
                    }

                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.Id))
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
            return View(reservation);
        }

        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Reservation == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Reservation == null)
            {
                return Problem("Entity set 'WebAppHotelContext.Reservation'  is null.");
            }
            var reservation = await _context.Reservation.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservation.Remove(reservation);

                StatusTimelineLog checkIn = (from log in _context.statusTimelineLog
                                            where log.Date == reservation.CheckIn
                                            select log).FirstOrDefault();

                StatusTimelineLog checkOut = (from log in _context.statusTimelineLog
                                             where log.Date == reservation.CheckOut
                                             select log).FirstOrDefault();

                _context.statusTimelineLog.Remove(checkIn);                    
                _context.statusTimelineLog.Remove(checkOut);

                Room room = _context.Room.Find(reservation.RoomId);
                if (DateTime.Now > checkIn.Date && DateTime.Now < checkOut.Date)
                {
                    room.Status = RoomStatus.Available;
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(int id)
        {
            return (_context.Reservation?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

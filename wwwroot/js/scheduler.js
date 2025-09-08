// Global variables
let selectedOfficeId = null;
let selectedSlot = null;
let offices = [];
let rooms = [];
let currentView = 'month';
let connection = null;
let currentUser = null;
let currentHubOfficeId = null;

// Initialize when page loads
document.addEventListener('DOMContentLoaded', function() {
    initializeScheduler();
    setupSignalR();
});

// Initialize the scheduler
async function initializeScheduler() {
    showLoading(true);
    try {
        await loadCurrentUser();
        await seedEmployees(); // Ensure default employees exist
        await loadOffices();
        await updateStats();
        setDefaultDate();
    } catch (error) {
        console.error('Failed to initialize scheduler:', error);
        showNotification('Failed to load scheduler data', 'error');
    } finally {
        showLoading(false);
    }
}

// Setup SignalR connection for real-time updates
function setupSignalR() {
    connection = new signalR.HubConnectionBuilder()
        .withUrl("/hubs/scheduler")
        .withAutomaticReconnect()
        .build();

    connection.start().then(function () {
        updateConnectionStatus(true);
        console.log('Connected to scheduler hub');
        
        // Join currently selected office group (if any)
        if (selectedOfficeId) {
            try { connection.invoke('JoinOfficeGroup', selectedOfficeId); currentHubOfficeId = selectedOfficeId; } catch (e) { }
        }
    }).catch(function (err) {
        updateConnectionStatus(false);
        console.error('Failed to connect to scheduler hub:', err);
    });

    // Handle real-time meeting updates
    connection.on("MeetingCreated", function (meeting) {
        showNotification(`New meeting scheduled: ${meeting.subject}`, 'success');
        refreshCurrentView();
        updateStats();
    });

    connection.on("MeetingUpdated", function (meeting) {
        showNotification(`Meeting updated: ${meeting.subject}`, 'info');
        refreshCurrentView();
        updateStats();
    });

    connection.on("MeetingCancelled", function (meetingId) {
        showNotification('Meeting has been cancelled', 'warning');
        refreshCurrentView();
        updateStats();
    });

    // Handle connection events
    connection.onreconnecting(() => updateConnectionStatus(false));
    connection.onreconnected(() => updateConnectionStatus(true));
    connection.onclose(() => updateConnectionStatus(false));
}

// Load current user information
async function loadCurrentUser() {
    try {
        // Try to get current user info
        const response = await fetch('/api/account/current-user', {
            credentials: 'include'
        });
        
        if (response.ok) {
            currentUser = await response.json();
            console.log('Current user loaded:', currentUser);
        } else {
            console.warn('Could not load current user info, loading available employees');
            
            // Fallback: get any available employee
            const empResponse = await fetch('/api/v1/employees', {
                credentials: 'include'
            });
            
            if (empResponse.ok) {
                const employees = await empResponse.json();
                if (employees && employees.length > 0) {
                    currentUser = { id: employees[0].id, name: employees[0].fullName };
                    console.log('Using first available employee as organizer:', currentUser);
                }
            }
        }
    } catch (error) {
        console.error('Failed to load current user:', error);
        
        // Last resort fallback: try to load employees
        try {
            const empResponse = await fetch('/api/v1/employees', {
                credentials: 'include'
            });
            
            if (empResponse.ok) {
                const employees = await empResponse.json();
                if (employees && employees.length > 0) {
                    currentUser = { id: employees[0].id, name: employees[0].fullName };
                    console.log('Using fallback employee as organizer:', currentUser);
                }
            }
        } catch (fallbackError) {
            console.error('Failed to load fallback employee:', fallbackError);
        }
    }
}

// Seed default employees if needed
async function seedEmployees() {
    try {
        await fetch('/api/v1/employees/seed', { 
            method: 'POST',
            credentials: 'include',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': getAntiForgeryToken()
            }
        });
        console.log('Employee seeding completed');
    } catch (error) {
        console.error('Failed to seed employees:', error);
    }
}

// Load offices from API
async function loadOffices() {
    try {
        // First try to seed offices if none exist
        await fetch('/api/v1/offices/seed-india', { 
            method: 'POST',
            credentials: 'include',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': getAntiForgeryToken()
            }
        });

        // Then load all offices
        const response = await fetch('/api/v1/offices', {
            credentials: 'include'
        });
        
        if (!response.ok) {
            throw new Error(`HTTP ${response.status}: ${response.statusText}`);
        }
        
    offices = await response.json();
        renderOfficeCards();
        populateOfficeDropdown();
    // No-op: will join a group when an office is selected
    } catch (error) {
        console.error('Failed to load offices:', error);
        showNotification('Failed to load offices', 'error');
    }
}

// Render office selection cards
function renderOfficeCards() {
    const container = document.getElementById('officesContainer');
    container.innerHTML = '';

    offices.forEach(office => {
        const card = document.createElement('div');
        card.className = 'col-md-4 mb-3';
        card.innerHTML = `
            <div class="card office-card" data-office-id="${office.id}" onclick="selectOffice('${office.id}', '${office.name}')">
                <div class="card-body text-center">
                    <h5 class="card-title">
                        <i class="fas fa-building"></i> ${office.name}
                    </h5>
                    <div class="office-stats">
                        <div>Timezone: ${office.timezone ?? office.timeZoneId ?? ''}</div>
                        <div>Hours: ${office.workingHours ?? ((office.businessStart||'') + ' - ' + (office.businessEnd||''))}</div>
                        <div>Days: ${office.workingDays ?? office.workDays ?? ''}</div>
                    </div>
                </div>
            </div>
        `;
        container.appendChild(card);
    });
}

// Populate office dropdown in modal
function populateOfficeDropdown() {
    const select = document.getElementById('meetingOffice');
    select.innerHTML = '<option value="">Select Office</option>';
    
    offices.forEach(office => {
        const option = document.createElement('option');
        option.value = office.id;
        option.textContent = office.name;
        select.appendChild(option);
    });
}

// Select an office and load its calendar
async function selectOffice(officeId, officeName) {
    selectedOfficeId = officeId;
    
    // Update UI
    document.querySelectorAll('.office-card').forEach(card => {
        card.classList.remove('selected');
    });
    const selectedCard = document.querySelector(`.office-card[data-office-id="${officeId}"]`);
    if (selectedCard) selectedCard.classList.add('selected');
    
    document.getElementById('selectedOfficeName').textContent = officeName;
    setCalendarTheme(officeName);

    // Manage SignalR group subscriptions per office
    try {
        if (connection) {
            if (currentHubOfficeId && currentHubOfficeId !== officeId) {
                try { await connection.invoke('LeaveOfficeGroup', currentHubOfficeId); } catch (e) { }
            }
            try { await connection.invoke('JoinOfficeGroup', officeId); currentHubOfficeId = officeId; } catch (e) { }
        }
    } catch (e) { /* ignore */ }
    
    // Load rooms for this office
    await loadRooms(officeId);
    
    // Load calendar for this office
    await loadCalendar();
    
    showNotification(`Switched to ${officeName} office`, 'info');
}

// Load rooms for selected office
async function loadRooms(officeId) {
    try {
    const response = await fetch(`/api/v1/offices/${officeId}/rooms`, {
            credentials: 'include'
        });
        
        if (response.ok) {
            rooms = await response.json();
            populateRoomDropdown();
        }
    } catch (error) {
        console.error('Failed to load rooms:', error);
    }
}

// Populate room dropdown in modal
function populateRoomDropdown() {
    const select = document.getElementById('meetingRoom');
    
    // Keep the default options and add API-loaded rooms
    const defaultOptions = `
        <option value="">Any available room</option>
    `;
    
    select.innerHTML = defaultOptions;
    
    // Add rooms from API if available
    if (rooms && rooms.length > 0) {
        rooms.forEach(room => {
            const option = document.createElement('option');
            option.value = room.id;
            option.textContent = `${room.name} (${room.capacity} people)`;
            select.appendChild(option);
        });
    }
}

// Load calendar view
async function loadCalendar() {
    if (!selectedOfficeId) return;
    
    const calendar = document.getElementById('calendar');
    calendar.innerHTML = `
        <div class="text-center text-muted mt-5">
            <i class="fas fa-spinner fa-spin fa-3x"></i>
            <h4>Loading meetings...</h4>
        </div>
    `;
    
    try {
    // Load meetings from now to end of current month (hide past events)
    const now = new Date();
    const startDate = new Date();
        const endDate = new Date(now.getFullYear(), now.getMonth() + 1, 0, 23, 59, 59, 999);
        
    const response = await fetch(`/api/v1/meetings/calendar?fromUtc=${startDate.toISOString()}&toUtc=${endDate.toISOString()}&officeId=${encodeURIComponent(selectedOfficeId)}&_t=${Date.now()}`, {
            credentials: 'include',
            cache: 'no-cache'
        });
        
        if (response.ok) {
            const meetings = await response.json();
            renderCalendar(meetings);
        } else {
            throw new Error('Failed to load meetings');
        }
    } catch (error) {
        console.error('Failed to load calendar:', error);
        calendar.innerHTML = `
            <div class="text-center text-danger mt-5">
                <i class="fas fa-exclamation-triangle fa-3x"></i>
                <h4>Failed to load meetings</h4>
                <p>Please try refreshing the page</p>
            </div>
        `;
    }
}

// Render calendar with meetings
function renderCalendar(meetings) {
    console.log('Rendering calendar with meetings:', meetings);
    const calendar = document.getElementById('calendar');
    const now = new Date();
    const currentMonth = now.getMonth();
    const currentYear = now.getFullYear();
    
    // Create calendar grid
    let calendarHTML = `
        <div class="calendar-header mb-3">
            <div class="row">
                <div class="col">
                    <h5>${now.toLocaleString('default', { month: 'long' })} ${currentYear}</h5>
                </div>
            </div>
        </div>
        <div class="calendar-grid">
            <div class="row calendar-weekdays">
                <div class="col text-center font-weight-bold">Sun</div>
                <div class="col text-center font-weight-bold">Mon</div>
                <div class="col text-center font-weight-bold">Tue</div>
                <div class="col text-center font-weight-bold">Wed</div>
                <div class="col text-center font-weight-bold">Thu</div>
                <div class="col text-center font-weight-bold">Fri</div>
                <div class="col text-center font-weight-bold">Sat</div>
            </div>
    `;
    
    // Get first day of month and days in month
    const firstDay = new Date(currentYear, currentMonth, 1).getDay();
    const daysInMonth = new Date(currentYear, currentMonth + 1, 0).getDate();
    
    // Create weeks
    let dayCount = 1;
    for (let week = 0; week < 6; week++) {
        calendarHTML += '<div class="row calendar-week border-bottom">';
        
        for (let day = 0; day < 7; day++) {
            if (week === 0 && day < firstDay) {
                calendarHTML += '<div class="col calendar-day p-2" style="min-height: 100px;"></div>';
            } else if (dayCount <= daysInMonth) {
                const nowTs = Date.now();
                const dayMeetings = meetings.filter(m => {
                    const startDate = new Date(m.startUtc);
                    const endDate = new Date(m.endUtc);
                    return startDate.getDate() === dayCount && 
                           startDate.getMonth() === currentMonth &&
                           startDate.getFullYear() === currentYear &&
                           endDate.getTime() > nowTs; // hide past-ended
                });

                const isToday = dayCount === now.getDate() && currentMonth === now.getMonth() && currentYear === now.getFullYear();
                const flagHtml = isToday ? '<span class="badge bg-primary ms-1">Today</span>' : '';

                calendarHTML += `
                    <div class="col calendar-day p-2 border-right${isToday ? ' today' : ''}" style="min-height: 100px;">
                        <div class="d-flex align-items-center justify-content-between">
                            <div class="day-number font-weight-bold">${dayCount}</div>
                            ${flagHtml}
                        </div>
                        <div class="day-meetings">
                `;
                
                const count = dayMeetings.length;
                if (count > 0) {
                    calendarHTML += `<div class="badge bg-secondary mb-1" title="${count} meetings">${count} meeting${count>1?'s':''}</div>`;
                }
                dayMeetings.forEach(meeting => {
                    const startTime = new Date(meeting.startUtc).toLocaleTimeString('en-US', { 
                        hour: '2-digit', 
                        minute: '2-digit',
                        hour12: false 
                    });
                    const endTime = new Date(meeting.endUtc).toLocaleTimeString('en-US', { 
                        hour: '2-digit', 
                        minute: '2-digit',
                        hour12: false 
                    });
                    const tooltip = `${startTime}-${endTime} | ${meeting.subject}`;
                    calendarHTML += `
                        <div class="meeting-item" title="${tooltip}">
                            ${startTime} ${meeting.subject.substring(0, 15)}${meeting.subject.length > 15 ? '...' : ''}
                        </div>
                    `;
                });
                
                calendarHTML += '</div></div>';
                dayCount++;
            } else {
                calendarHTML += '<div class="col calendar-day p-2" style="min-height: 100px;"></div>';
            }
        }
        
        calendarHTML += '</div>';
        
        if (dayCount > daysInMonth) break;
    }
    
    calendarHTML += '</div>';
    calendar.innerHTML = calendarHTML;
}

// Update statistics
async function updateStats() {
    try {
        // Today
        const todayResp = await fetch(`/api/v1/meetings/stats/today?officeId=${selectedOfficeId??''}`, { credentials: 'include', cache: 'no-cache' });
        if (todayResp.ok) {
            const data = await todayResp.json();
            document.getElementById('todayMeetingsCount').textContent = data.todayMeetings ?? '0';
        }
        // Week
        const weekResp = await fetch(`/api/v1/meetings/stats/week?officeId=${selectedOfficeId??''}`, { credentials: 'include', cache: 'no-cache' });
        if (weekResp.ok) {
            const data = await weekResp.json();
            document.getElementById('weekMeetingsCount').textContent = data.weekMeetings ?? '0';
        }
        // Rooms available = active rooms - overlapping meetings now
        const now = new Date();
        const availResp = await fetch(`/api/v1/meetings/availability?officeId=${selectedOfficeId??''}&date=${now.toISOString().slice(0,10)}`, { credentials: 'include', cache: 'no-cache' });
        if (availResp.ok) {
            const { availability } = await availResp.json();
            const activeRooms = availability?.length ?? 0;
            // booked now
            const nowUtc = now.toISOString();
            const bookedNow = availability?.filter(r => r.bookings.some(b => b.startUtc < nowUtc && b.endUtc > nowUtc)).length ?? 0;
            const free = Math.max(activeRooms - bookedNow, 0);
            document.getElementById('availableRoomsCount').textContent = String(free);
        } else {
            document.getElementById('availableRoomsCount').textContent = rooms.length || '0';
        }
        document.getElementById('activeOfficesCount').textContent = offices.length || '0';
    } catch (error) {
        console.error('Failed to update stats:', error);
    }
}

// Show create meeting modal
function showCreateMeetingModal() {
    console.log('showCreateMeetingModal called');
    
    // Reset form
    document.getElementById('createMeetingForm').reset();
    document.getElementById('proposedSlots').style.display = 'none';
    document.getElementById('confirmMeetingBtn').style.display = 'none';
    selectedSlot = null;
    
    // Show modal - try Bootstrap 5 first, fallback to jQuery/Bootstrap 4
    const modalElement = document.getElementById('createMeetingModal');
    console.log('Modal element found:', modalElement);
    
    if (typeof bootstrap !== 'undefined' && bootstrap.Modal) {
        // Bootstrap 5
        console.log('Using Bootstrap 5 modal');
        const modal = new bootstrap.Modal(modalElement);
        modal.show();
    } else if (typeof $ !== 'undefined') {
        // Bootstrap 4 with jQuery
        console.log('Using jQuery/Bootstrap 4 modal');
        $(modalElement).modal('show');
    } else {
        // Fallback - direct style manipulation
        console.log('Using fallback modal display');
        modalElement.style.display = 'block';
        modalElement.classList.add('show');
        document.body.classList.add('modal-open');
        
        // Create backdrop
        const backdrop = document.createElement('div');
        backdrop.className = 'modal-backdrop fade show';
        backdrop.id = 'modal-backdrop';
        document.body.appendChild(backdrop);
    }
}

// Close modal function for fallback
function closeModal() {
    const modalElement = document.getElementById('createMeetingModal');
    const backdrop = document.getElementById('modal-backdrop');
    
    modalElement.style.display = 'none';
    modalElement.classList.remove('show');
    document.body.classList.remove('modal-open');
    
    if (backdrop) {
        backdrop.remove();
    }
}

// Propose available slots
async function proposeSlots() {
    const subject = document.getElementById('meetingSubject').value;
    const officeId = document.getElementById('meetingOffice').value || selectedOfficeId;
    const date = document.getElementById('meetingDate').value;
    const duration = document.getElementById('meetingDuration').value; // as minutes string
    const roomId = document.getElementById('meetingRoom').value;

    if (!subject || !officeId || !date || !duration) {
        showNotification('Please fill in all required fields', 'warning');
        return;
    }
    
    showLoading(true);
    
    try {
        const payload = {
            officeId: officeId,
            date: new Date(date).toISOString(),
            duration: String(duration),
            preferredRoom: roomId || '',
            organizerId: (currentUser && currentUser.id) ? currentUser.id : ''
        };
        const resp = await fetch('/api/v1/meetings/propose', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': getAntiForgeryToken() },
            credentials: 'include',
            body: JSON.stringify(payload)
        });
        if (!resp.ok) throw new Error('Failed to propose');
        const data = await resp.json();
        const slots = (data.slots || []).map(s => ({
            startUtc: s.startUtc || s.start,
            endUtc: s.endUtc || s.end,
            available: s.available,
            roomName: s.room || roomId || 'TBD',
            officeId: officeId,
            roomId: roomId || null
        }));

        // Slide transition
        const panel = document.getElementById('proposedSlots');
        panel.style.display = 'block';
        panel.style.transform = 'translateX(100%)';
        setTimeout(() => { panel.style.transform = 'translateX(0)'; }, 20);

        renderProposedSlots(slots);
        
    } catch (error) {
        console.error('Failed to propose slots:', error);
        showNotification('Failed to propose slots', 'error');
    } finally {
        showLoading(false);
    }
}

// Render proposed time slots
function renderProposedSlots(slots) {
    const container = document.getElementById('slotsList');
    container.innerHTML = '';
    
    if (slots.length === 0) {
        container.innerHTML = '<div class="alert alert-warning">No available slots found for the selected criteria.</div>';
    } else {
        slots.forEach((slot, index) => {
            const slotDiv = document.createElement('div');
            slotDiv.className = 'slot-option' + (slot.available ? '' : ' disabled');
            if (slot.available) slotDiv.onclick = () => selectSlot(index, slot);
            
            const startTime = new Date(slot.startUtc).toLocaleTimeString('en-US', { 
                hour: '2-digit', 
                minute: '2-digit' 
            });
            const endTime = new Date(slot.endUtc).toLocaleTimeString('en-US', { 
                hour: '2-digit', 
                minute: '2-digit' 
            });
            
            slotDiv.innerHTML = `
                <strong>${startTime} - ${endTime}</strong> ${slot.available ? '' : '<span class="badge bg-danger ms-2">Booked</span>'}<br>
                <small class="text-muted">Room: ${slot.roomName || 'TBD'}</small>
            `;
            
            container.appendChild(slotDiv);
        });
    }
    
    document.getElementById('proposedSlots').style.display = 'block';
}

// Select a time slot
function selectSlot(index, slot) {
    // Remove previous selection
    document.querySelectorAll('.slot-option').forEach(el => el.classList.remove('selected'));
    
    // Select current slot
    document.querySelectorAll('.slot-option')[index].classList.add('selected');
    selectedSlot = slot;
    
    // Show confirm button
    document.getElementById('confirmMeetingBtn').style.display = 'inline-block';
}

// Confirm and create meeting
async function confirmMeeting() {
    if (!selectedSlot) {
        showNotification('Please select a time slot', 'warning');
        return;
    }
    
    showLoading(true);
    
    try {
        // Parse attendee emails
        const attendeeEmails = document.getElementById('meetingAttendees').value
            .split(',')
            .map(email => email.trim())
            .filter(email => email.length > 0);
            
        // Use current user ID or fallback to null (let backend handle it)
        let organizerId = null;
        if (currentUser && currentUser.id) {
            organizerId = currentUser.id;
        } else {
            showNotification('Please wait while we set up your organizer profile...', 'error');
            return; // Don't proceed without a valid organizer
        }
        
        const meetingData = {
            subject: document.getElementById('meetingSubject').value,
            description: document.getElementById('meetingDescription').value,
            organizerId: organizerId,
            officeId: selectedOfficeId,
            roomId: selectedSlot.roomId || null,
            start: selectedSlot.startUtc,
            end: selectedSlot.endUtc,
            organizerName: document.getElementById('meetingOrganizer')?.value || (currentUser?.name ?? ''),
            participantIds: [] // We'll need to resolve emails to employee IDs in the backend
        };
        
        console.log('Creating meeting with data:', meetingData);
        
        const response = await fetch('/api/v1/meetings', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': getAntiForgeryToken()
            },
            credentials: 'include',
            body: JSON.stringify(meetingData)
        });
        
        if (response.ok) {
            const meeting = await response.json();
            showNotification('Meeting scheduled successfully!', 'success');
            closeCreateMeetingModal();
            
            // Add a small delay to ensure the meeting is fully created before refreshing
            setTimeout(() => {
                refreshCurrentView();
                updateStats();
            }, 500);
        } else {
            const errorText = await response.text();
            console.error('Meeting creation failed:', errorText);
            showNotification(`Failed to create meeting: ${errorText}`, 'error');
        }
    } catch (error) {
        console.error('Failed to create meeting:', error);
        showNotification('Failed to create meeting', 'error');
    } finally {
        showLoading(false);
    }
}

// Close modal function
function closeCreateMeetingModal() {
    const modalElement = document.getElementById('createMeetingModal');
    
    if (typeof bootstrap !== 'undefined' && bootstrap.Modal) {
        const modal = bootstrap.Modal.getInstance(modalElement);
        if (modal) modal.hide();
    } else if (typeof $ !== 'undefined') {
        $(modalElement).modal('hide');
    } else {
        closeModal();
    }
}

// Utility functions
function setDefaultDate() {
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    document.getElementById('meetingDate').value = tomorrow.toISOString().split('T')[0];
}

function refreshData() {
    refreshCurrentView();
    updateStats();
    showNotification('Data refreshed', 'info');
}

function refreshCurrentView() {
    if (selectedOfficeId) {
        showLoading(true);
        loadCalendar().finally(() => {
            showLoading(false);
        });
    }
}

function changeView(view, evt) {
    currentView = view;
    // Update button states (guard against missing event/target)
    try {
        const buttons = document.querySelectorAll('.btn-group button');
        if (buttons && buttons.length) {
            buttons.forEach(btn => {
                btn.classList.remove('btn-secondary');
                btn.classList.add('btn-outline-secondary');
            });
        }
        const target = (evt && evt.target) ? evt.target : (typeof event !== 'undefined' && event.target ? event.target : null);
        if (target) {
            target.classList.remove('btn-outline-secondary');
            target.classList.add('btn-secondary');
        }
    } catch (_) { /* noop */ }

    // Refresh calendar with new view
    refreshCurrentView();
}

// Set per-office calendar theme (visual distinction only)
function setCalendarTheme(officeName) {
    const el = document.getElementById('calendar');
    if (!el) return;
    // Remove previous office-* classes
    el.className = el.className
        .split(' ')
        .filter(c => !c.startsWith('office-'))
        .join(' ');
    const slug = (officeName || '').toLowerCase().replace(/[^a-z0-9]+/g, '-').replace(/(^-|-$)/g, '');
    if (slug) {
        el.classList.add(`office-${slug}`);
    }
}

// Open availability popup
async function showAvailability() {
        if (!selectedOfficeId) { showNotification('Select an office first', 'warning'); return; }
        const today = new Date().toISOString().slice(0,10);
        const resp = await fetch(`/api/v1/meetings/availability?officeId=${selectedOfficeId}&date=${today}`, { credentials: 'include' });
        if (!resp.ok) return;
        const data = await resp.json();
        const modal = document.createElement('div');
        modal.className = 'modal fade show';
        modal.style.display = 'block';
        modal.innerHTML = `
            <div class="modal-dialog modal-lg"><div class="modal-content">
                <div class="modal-header"><h5 class="modal-title">Room Availability</h5>
                    <button type="button" class="btn-close" onclick="this.closest('.modal').remove()"></button></div>
                <div class="modal-body">
                    ${data.availability.map(r => renderRoomAvailability(r)).join('')}
                </div>
            </div></div>`;
        document.body.appendChild(modal);
}

function renderRoomAvailability(room) {
        // Build hour slots 10-18
        const hours = Array.from({length:8}, (_,i)=> i+10);
        let rows = '<div class="mb-2"><strong>'+room.roomName+'</strong></div><div class="d-flex flex-wrap">';
        hours.forEach(h => {
                const start = new Date(); start.setHours(h,0,0,0);
                const end = new Date(); end.setHours(h+1,0,0,0);
                const booked = room.bookings.some(b => (new Date(b.startUtc)) < end && (new Date(b.endUtc)) > start);
                rows += `<div class="badge ${booked?'bg-danger':'bg-success'} me-2 mb-2" style="min-width:90px;">${h}:00-${h+1}:00 ${booked?'Booked':'Free'}</div>`;
        });
        rows += '</div><hr/>';
        return rows;
}

function showLoading(show) {
    const overlay = document.getElementById('loadingOverlay');
    if (show) {
        overlay.classList.remove('d-none');
    } else {
        overlay.classList.add('d-none');
    }
}

function showNotification(message, type = 'info') {
    // Create toast/alert notification (Bootstrap 5 compatible)
    const toast = document.createElement('div');
    toast.className = `alert alert-${type === 'error' ? 'danger' : type} alert-dismissible fade show position-fixed`;
    toast.style.cssText = 'top: 20px; right: 20px; z-index: 9999; min-width: 300px;';
    toast.innerHTML = `
        <div class="d-flex justify-content-between align-items-start">
            <div class="me-3">${message}</div>
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        </div>
    `;

    document.body.appendChild(toast);

    // Auto remove after 5 seconds
    setTimeout(() => {
        if (toast.parentNode) {
            toast.parentNode.removeChild(toast);
        }
    }, 5000);
}

function updateConnectionStatus(connected) {
    const status = document.getElementById('connectionStatus');
    if (!status) return;
    if (connected) {
        status.innerHTML = '<span class="badge bg-success"><i class="fas fa-wifi"></i> Connected</span>';
    } else {
        status.innerHTML = '<span class="badge bg-danger"><i class="fas fa-wifi"></i> Disconnected</span>';
    }
}

function getAntiForgeryToken() {
    const token = document.querySelector('input[name="__RequestVerificationToken"]');
    return token ? token.value : '';
}

// Periodic refresh (every minute)
setInterval(() => {
    refreshCurrentView();
    updateStats();
}, 60000);

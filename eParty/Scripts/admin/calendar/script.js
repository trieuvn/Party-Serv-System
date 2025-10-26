// Đảm bảo script chạy sau khi DOM đã tải hoàn chỉnh
document.addEventListener('DOMContentLoaded', function () {
    // DOM Elements
    const calendarView = document.getElementById('calendar-view');
    const eventsList = document.getElementById('events-list');
    const currentDateElement = document.getElementById('current-date');
    const todayBtn = document.getElementById('today-btn');
    const prevBtn = document.getElementById('prev-btn');
    const nextBtn = document.getElementById('next-btn');
    const viewOptions = document.querySelectorAll('.view-option');
    const addEventBtn = document.getElementById('add-event-btn');
    const eventModal = document.getElementById('event-modal');
    const eventDetailsModal = document.getElementById('event-details-modal');
    const closeBtns = document.querySelectorAll('.close-btn');
    const eventForm = document.getElementById('event-form');
    const eventTitleInput = document.getElementById('event-title');
    const eventDateInput = document.getElementById('event-date');
    const eventStartTimeInput = document.getElementById('event-start-time');
    const eventEndTimeInput = document.getElementById('event-end-time');
    const eventDescriptionInput = document.getElementById('event-description');
    const eventColorInput = document.getElementById('event-color');
    const eventReminderInput = document.getElementById('event-reminder');
    const detailsTitle = document.getElementById('details-title');
    const detailsDate = document.getElementById('details-date');
    const detailsTime = document.getElementById('details-time');
    const detailsDescription = document.getElementById('details-description');
    const deleteEventBtn = document.getElementById('delete-event-btn');
    const editEventBtn = document.getElementById('edit-event-btn');
    const modalTitle = document.getElementById('modal-title');
    const eventIdInput = document.getElementById('event-id');

    // App State
    let currentView = 'month';
    let currentDate = new Date();
    let events = (typeof window.serverEvents !== 'undefined' && Array.isArray(window.serverEvents))
        ? window.serverEvents
        : (JSON.parse(localStorage.getItem('events')) || []);
    let selectedEventId = null;

    // *** SỬA LỖI (Patch #4): Đọc chiều cao từ biến CSS --hour-h ***
    // (Lưu ý: Sẽ đọc sau khi init() để đảm bảo CSS đã tải)
    let pxPerHour = 50; // Giá trị fallback

    // Initialize
    function init() {
        // Đọc giá trị pxPerHour sau khi DOM load
        pxPerHour = parseInt(getComputedStyle(document.documentElement).getPropertyValue('--hour-h') || '50');

        console.log("Initializing calendar with events:", events, "Hour Height:", pxPerHour);
        renderCalendar();
        renderEventsList();
        setupEventListeners();
        requestNotificationPermission();
    }

    // Setup Event Listeners
    function setupEventListeners() {
        todayBtn?.addEventListener('click', goToToday);
        prevBtn?.addEventListener('click', navigatePrevious);
        nextBtn?.addEventListener('click', navigateNext);
        viewOptions.forEach(option => option.addEventListener('click', () => switchView(option.dataset.view)));
        addEventBtn?.addEventListener('click', () => openEventModal());
        closeBtns.forEach(btn => btn.addEventListener('click', closeModals));
        eventForm?.addEventListener('submit', saveOrUpdateEvent);
        deleteEventBtn?.addEventListener('click', deleteEvent);

        editEventBtn?.addEventListener('click', () => {
            if (!selectedEventId) return;
            if (eventDetailsModal) eventDetailsModal.style.display = 'none';
            openEventModal(selectedEventId);
        });

        window.addEventListener('click', (e) => { if (e.target === eventModal || e.target === eventDetailsModal) closeModals(); });
        const toggleBtn = document.getElementById('toggle-sidebar'); const sidebar = document.getElementById('event-sidebar');
        if (toggleBtn && sidebar) { toggleBtn.addEventListener('click', () => sidebar.classList.toggle('open')); }
        else { console.warn("Sidebar toggle button or sidebar element not found."); }
    }

    // Render Main Calendar View
    function renderCalendar() {
        if (!calendarView) { console.error("Calendar view element not found!"); return; }
        calendarView.innerHTML = '';
        switch (currentView) {
            case 'day': renderDayView(); break;
            case 'week': renderWeekView(); break;
            case 'month': default: renderMonthView(); break;
        }
        updateCurrentDateDisplay();
    }

    // --- Render Month View (Giữ nguyên) ---
    function renderMonthView() {
        const monthContainer = document.createElement('div'); monthContainer.className = 'month-view'; const firstDayOfMonth = new Date(currentDate.getFullYear(), currentDate.getMonth(), 1); const lastDayOfMonth = new Date(currentDate.getFullYear(), currentDate.getMonth() + 1, 0); const daysInMonth = lastDayOfMonth.getDate(); const startingDayOfWeek = firstDayOfMonth.getDay(); const monthHeader = document.createElement('div'); monthHeader.className = 'month-header';['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'].forEach(day => { const dayElement = document.createElement('div'); dayElement.className = 'day-header'; dayElement.textContent = day; monthHeader.appendChild(dayElement); }); monthContainer.appendChild(monthHeader); const daysGrid = document.createElement('div'); daysGrid.className = 'month-days'; const daysInPrevMonth = new Date(currentDate.getFullYear(), currentDate.getMonth(), 0).getDate(); for (let i = 0; i < startingDayOfWeek; i++) { const dayDate = new Date(currentDate.getFullYear(), currentDate.getMonth() - 1, daysInPrevMonth - startingDayOfWeek + i + 1); daysGrid.appendChild(createDayCell(dayDate, true)); } const today = new Date(); for (let i = 1; i <= daysInMonth; i++) { const dayDate = new Date(currentDate.getFullYear(), currentDate.getMonth(), i); const isToday = dayDate.toDateString() === today.toDateString(); daysGrid.appendChild(createDayCell(dayDate, false, isToday)); } const totalCells = startingDayOfWeek + daysInMonth; const remainingCells = (Math.ceil(totalCells / 7) * 7) - totalCells; for (let i = 1; i <= remainingCells; i++) { const dayDate = new Date(currentDate.getFullYear(), currentDate.getMonth() + 1, i); daysGrid.appendChild(createDayCell(dayDate, true)); } monthContainer.appendChild(daysGrid); calendarView.appendChild(monthContainer);
    }
    function createDayCell(date, isOtherMonth, isToday = false) {
        const dayCell = document.createElement('div'); dayCell.className = `day-cell ${isOtherMonth ? 'other-month' : ''} ${isToday ? 'current-day' : ''}`; const dayNumber = document.createElement('div'); dayNumber.className = 'day-number'; dayNumber.textContent = date.getDate(); dayCell.appendChild(dayNumber); const dayEventsContainer = document.createElement('div'); dayEventsContainer.className = 'day-events'; const dayEvents = getEventsForDate(date); const maxEventsToShow = 2; let eventsShownCount = 0; dayEvents.forEach(event => { if (eventsShownCount < maxEventsToShow) { const eventElement = document.createElement('div'); eventElement.className = 'day-event'; eventElement.textContent = event.title; eventElement.style.backgroundColor = event.color; eventElement.dataset.eventId = event.id; eventElement.addEventListener('click', (e) => { e.stopPropagation(); showEventDetails(event.id); }); dayEventsContainer.appendChild(eventElement); eventsShownCount++; } }); if (dayEvents.length > maxEventsToShow) { const moreEvents = document.createElement('div'); moreEvents.className = 'more-events'; moreEvents.textContent = `+${dayEvents.length - maxEventsToShow} more`; dayEventsContainer.appendChild(moreEvents); } dayCell.appendChild(dayEventsContainer); dayCell.addEventListener('click', () => { currentDate = new Date(date); if (isOtherMonth) { renderCalendar(); } else { switchView('day'); } }); return dayCell;
    }

    // --- SỬA LỖI DAY VIEW DURATION (Patch #2 - đã áp dụng) ---
    function renderDayView() {
        const dayContainer = document.createElement('div'); dayContainer.className = 'day-view';
        //const dayHeader = document.createElement('div'); dayHeader.className = 'day-header-full'; dayHeader.innerHTML = `<h2>${currentDate.toLocaleDateString('en-US', { weekday: 'long', month: 'long', day: 'numeric', year: 'numeric' })}</h2>`; dayContainer.appendChild(dayHeader);
        const dayGrid = document.createElement('div'); dayGrid.className = 'day-grid';

        for (let hour = 0; hour < 24; hour++) {
            const hourLabel = document.createElement('div'); hourLabel.className = 'hour-label'; const displayHour = hour === 0 ? 12 : hour > 12 ? hour - 12 : hour; const ampm = hour < 12 ? 'AM' : 'PM'; hourLabel.textContent = `${displayHour} ${ampm}`;
            dayGrid.appendChild(hourLabel);
            const timeBlock = document.createElement('div'); timeBlock.className = 'day-time-block';
            dayGrid.appendChild(timeBlock);
            timeBlock.addEventListener('click', () => openEventModalForTime(currentDate, hour));
        }

        const overlay = document.createElement('div');
        overlay.className = 'events-overlay';
        dayContainer.appendChild(overlay); // đặt overlay ra ngoài grid
        overlay.style.position = 'absolute';
        overlay.style.top = '0';
        overlay.style.left = '60px'; // đúng cột timeline sau cột giờ
        overlay.style.right = '0';
        overlay.style.bottom = '0';
        overlay.style.pointerEvents = 'none';
        dayContainer.style.position = 'relative';

        dayContainer.appendChild(dayGrid);
        calendarView.appendChild(dayContainer);

        // *** SỬA LỖI (Patch #2 - Đo chiều cao thật) ***
        // Đo chiều cao 1 ô giờ (sau khi đã append vào DOM)
        const probe = dayGrid.querySelector('.day-time-block');
        const hourH = probe ? probe.getBoundingClientRect().height : pxPerHour; // Dùng pxPerHour làm fallback
        const pxPerMinute = hourH / 60; // Tính pixel mỗi phút
        // *** KẾT THÚC SỬA LỖI ***

        const todaysEvents = events.filter(ev => { try { const s = new Date(ev.startTime); return s.toDateString() === currentDate.toDateString(); } catch (e) { return false; } });

        todaysEvents.forEach(ev => {
            try {
                const s = new Date(ev.startTime), eDate = new Date(ev.endTime); if (isNaN(s.getTime()) || isNaN(eDate.getTime())) throw new Error("Invalid date");
                const minutesFromStartOfDay = s.getHours() * 60 + s.getMinutes();
                const durationMinutes = Math.max(15, (eDate.getTime() - s.getTime()) / 60000);

                const el = document.createElement('div'); el.className = 'day-event-abs';
                el.textContent = `${formatTime(s)} - ${ev.title}`;
                el.style.position = 'absolute';

                // *** SỬA LỖI (Patch #2): Tính toán dựa trên chiều cao đo được ***
                el.style.top = (minutesFromStartOfDay * pxPerMinute) + 'px';
                el.style.height = (durationMinutes * pxPerMinute) + 'px';

                el.style.background = ev.color;
                el.dataset.eventId = ev.id;
                el.title = `${formatTime(s)} - ${formatTime(eDate)}: ${ev.title}`;
                el.addEventListener('click', (e) => { e.stopPropagation(); showEventDetails(ev.id); });
                overlay.appendChild(el);
            } catch (error) { console.error("Error rendering day event:", ev, error); }
        });
    }

    // --- SỬA LỖI WEEK VIEW TRỐNG (Patch #3 - đã áp dụng) ---
    function renderWeekView() {
        const weekContainer = document.createElement('div'); weekContainer.className = 'week-view';
        const startOfWeek = new Date(currentDate); startOfWeek.setDate(currentDate.getDate() - currentDate.getDay()); startOfWeek.setHours(0, 0, 0, 0);
        const header = document.createElement('div'); header.className = 'week-header'; weekContainer.appendChild(header);
        const corner = document.createElement('div'); corner.className = 'hour-label-corner'; header.appendChild(corner);
        const grid = document.createElement('div'); grid.className = 'week-grid';

        for (let hour = 0; hour < 24; hour++) {
            const hourLabel = document.createElement('div'); hourLabel.className = 'hour-label'; const displayHour = hour === 0 ? 12 : hour > 12 ? hour - 12 : hour; const ampm = hour < 12 ? 'AM' : 'PM'; hourLabel.textContent = `${displayHour} ${ampm}`;
            grid.appendChild(hourLabel);
            for (let d = 0; d < 7; d++) {
                const timeBlock = document.createElement('div'); timeBlock.className = 'week-time-block';
                const blockDate = new Date(startOfWeek); blockDate.setDate(startOfWeek.getDate() + d);
                timeBlock.dataset.date = ymdLocal(blockDate); // Dùng YYYY-MM-DD local
                timeBlock.dataset.hour = hour;
                timeBlock.addEventListener('click', () => openEventModalForTime(blockDate, hour)); grid.appendChild(timeBlock);
            }
        }

        weekContainer.appendChild(grid); // Thêm grid vào DOM TRƯỚC khi đo
        calendarView.appendChild(weekContainer); // Thêm container vào DOM

        // *** SỬA LỖI (Patch #3 - Đo chiều cao thật) ***
        const probeWeek = grid.querySelector('.week-time-block');
        const hourHWeek = probeWeek ? probeWeek.getBoundingClientRect().height : pxPerHour;
        const ppmWeek = hourHWeek / 60; // px Per Minute Week
        // *** KẾT THÚC SỬA LỖI ***

        const endOfWeek = new Date(startOfWeek); endOfWeek.setDate(startOfWeek.getDate() + 7);
        const weekEvents = events.filter(ev => { try { const s = new Date(ev.startTime); return s >= startOfWeek && s < endOfWeek; } catch (e) { return false; } });
        const today = new Date(); today.setHours(0, 0, 0, 0);

        for (let d = 0; d < 7; d++) {
            const dayDate = new Date(startOfWeek); dayDate.setDate(startOfWeek.getDate() + d);
            const h = document.createElement('div'); h.className = 'week-day-header'; if (dayDate.getTime() === today.getTime()) h.classList.add('current-day');
            h.innerHTML = `<div>${['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'][d]}</div><div class="week-day-number">${dayDate.getDate()}</div>`; header.appendChild(h);
            const currentDayStart = new Date(dayDate); const currentDayEnd = new Date(dayDate); currentDayEnd.setDate(currentDayStart.getDate() + 1);
            const dayEvents = weekEvents.filter(ev => { try { const s = new Date(ev.startTime); return s >= currentDayStart && s < currentDayEnd; } catch (e) { return false; } });

            dayEvents.forEach(ev => {
                try {
                    const s = new Date(ev.startTime), eDate = new Date(ev.endTime); if (isNaN(s.getTime()) || isNaN(eDate.getTime())) throw new Error("Invalid Date");
                    const startHour = s.getHours(); const startMinutes = s.getMinutes(); const durationMinutes = Math.max(15, (eDate.getTime() - s.getTime()) / 60000);
                    const keyDate = ymdLocal(s);
                    const targetBlock = grid.querySelector(`.week-time-block[data-date="${keyDate}"][data-hour="${startHour}"]`);

                    if (targetBlock) {
                        const el = document.createElement('div'); el.className = 'week-event'; el.style.position = 'absolute';

                        // *** SỬA LỖI (Patch #3): Tính toán dựa trên chiều cao đo được ***
                        el.style.top = (startMinutes * ppmWeek) + 'px';
                        const eventHeight = durationMinutes * ppmWeek;
                        // Giới hạn chiều cao tối đa là 24h trừ đi vị trí bắt đầu
                        const maxHeight = (24 * hourHWeek) - (startMinutes * ppmWeek);
                        el.style.height = Math.min(eventHeight, maxHeight) + 'px';

                        el.style.background = ev.color;
                        el.textContent = `${formatTime(s)} ${ev.title}`; el.title = `${formatTime(s)} - ${formatTime(eDate)}: ${ev.title}`; el.dataset.eventId = ev.id;
                        el.addEventListener('click', (e) => { e.stopPropagation(); showEventDetails(ev.id); }); targetBlock.appendChild(el);
                    } else { console.warn(`Could not find target block for week event: ${ev.title} on ${keyDate} hr ${startHour}`); }
                } catch (error) { console.error("Error rendering week event:", ev, error); }
            });
        }
        // calendarView.appendChild(weekContainer); // Đã move lên trên
    }


    // --- Render Sidebar List (Giữ nguyên) ---
    function renderEventsList() {
        if (!eventsList) { return; }
        eventsList.innerHTML = '';
        const today = new Date(); today.setHours(0, 0, 0, 0);
        const upcomingEvents = events
            .filter(event => { try { return new Date(event.endTime) >= today; } catch (e) { return false; } })
            .sort((a, b) => { try { return new Date(a.startTime) - new Date(b.startTime); } catch (e) { return 0; } });

        if (upcomingEvents.length === 0) { eventsList.innerHTML = '<p class="text-muted text-center mt-3">No upcoming events.</p>'; return; }

        upcomingEvents.forEach(event => {
            const eventElement = document.createElement('div');
            eventElement.className = 'event-item mb-2 p-2 rounded'; eventElement.style.borderLeft = `5px solid ${event.color}`; eventElement.style.backgroundColor = '#f8f9fa'; eventElement.style.cursor = 'pointer'; eventElement.dataset.eventId = event.id;
            let startDate, endDate, formattedTime = "Invalid Date";
            try {
                startDate = new Date(event.startTime); endDate = new Date(event.endTime);
                if (isNaN(startDate.getTime()) || isNaN(endDate.getTime())) throw new Error("Invalid date");
                formattedTime = formatDateTime(startDate, endDate);
            } catch (e) { console.error("Error parsing date for list:", event); }
            eventElement.innerHTML = `<div class="d-flex justify-content-between align-items-center"><strong class="event-title">${event.title}</strong><span style="color: ${event.color}; font-size: 1.2em;">●</span></div><div class="event-time small text-muted">${formattedTime}</div>${event.description ? `<div class="event-description small mt-1">${event.description.substring(0, 50)}${event.description.length > 50 ? '...' : ''}</div>` : ''}`;
            eventsList.appendChild(eventElement);
            eventElement.addEventListener('click', () => { showEventDetails(event.id); });
        });
    }

    // --- Helper Functions for Event Filtering ---
    function getEventsForDate(date) {
        const targetStart = new Date(date.getFullYear(), date.getMonth(), date.getDate());
        const targetEnd = new Date(date.getFullYear(), date.getMonth(), date.getDate() + 1);
        return events.filter(event => {
            if (!event || !event.startTime) return false;
            try {
                const eventStartDate = new Date(event.startTime); if (isNaN(eventStartDate.getTime())) return false;
                return eventStartDate >= targetStart && eventStartDate < targetEnd;
            } catch (e) { console.error("Error filtering event date:", e); return false; }
        });
    }

    function ymdLocal(d) {
        if (!(d instanceof Date) || isNaN(d.getTime())) return "";
        const y = d.getFullYear();
        const m = String(d.getMonth() + 1).padStart(2, '0');
        const day = String(d.getDate()).padStart(2, '0');
        return `${y}-${m}-${day}`;
    }

    // --- UI Update, Navigation, Modals, CRUD, Storage, Reminders, Formatting ---
    function updateCurrentDateDisplay() {
        if (!currentDateElement) return; let options = {}; switch (currentView) { case 'day': options = { weekday: 'long', month: 'long', day: 'numeric', year: 'numeric' }; break; case 'week': const startOfWeek = new Date(currentDate); startOfWeek.setDate(currentDate.getDate() - currentDate.getDay()); startOfWeek.setHours(0, 0, 0, 0); const endOfWeek = new Date(startOfWeek); endOfWeek.setDate(startOfWeek.getDate() + 6); endOfWeek.setHours(23, 59, 59, 999); const startMonth = startOfWeek.toLocaleDateString('en-US', { month: 'short' }); const endMonth = endOfWeek.toLocaleDateString('en-US', { month: 'short' }); const startYear = startOfWeek.getFullYear(); const endYear = endOfWeek.getFullYear(); let dateString = `${startMonth} ${startOfWeek.getDate()}`; if (startYear !== endYear) dateString += `, ${startYear}`; dateString += ` - `; if (startMonth !== endMonth || startYear !== endYear) dateString += `${endMonth} `; dateString += `${endOfWeek.getDate()}`; dateString += `, ${endYear}`; currentDateElement.textContent = dateString; return; case 'month': default: options = { month: 'long', year: 'numeric' }; break; } currentDateElement.textContent = currentDate.toLocaleDateString('en-US', options);
    }
    function switchView(view) {
        if (!view || currentView === view) return; currentView = view; viewOptions.forEach(option => { option.classList.toggle('active', option.dataset.view === view); }); renderCalendar();
    }
    function navigatePrevious() {
        switch (currentView) { case 'day': currentDate.setDate(currentDate.getDate() - 1); break; case 'week': currentDate.setDate(currentDate.getDate() - 7); break; case 'month': currentDate.setMonth(currentDate.getMonth() - 1); break; } renderCalendar();
    }
    function navigateNext() {
        switch (currentView) { case 'day': currentDate.setDate(currentDate.getDate() + 1); break; case 'week': currentDate.setDate(currentDate.getDate() + 7); break; case 'month': currentDate.setMonth(currentDate.getMonth() + 1); break; } renderCalendar();
    }
    function goToToday() {
        currentDate = new Date(); renderCalendar();
    }
    function openEventModal(eventIdToEdit = null) {
        if (!eventModal || !eventForm) return; eventForm.reset(); eventForm.onsubmit = saveOrUpdateEvent; if (eventIdToEdit) { const event = events.find(e => e.id === eventIdToEdit); if (!event) return; modalTitle.textContent = "Edit Event"; eventIdInput.value = event.id; eventTitleInput.value = event.title; try { const startDate = new Date(event.startTime); const endDate = new Date(event.endTime); eventDateInput.value = startDate.toISOString().split('T')[0]; eventStartTimeInput.value = `${startDate.getHours().toString().padStart(2, '0')}:${startDate.getMinutes().toString().padStart(2, '0')}`; eventEndTimeInput.value = `${endDate.getHours().toString().padStart(2, '0')}:${endDate.getMinutes().toString().padStart(2, '0')}`; } catch (e) { eventDateInput.valueAsDate = new Date(); eventStartTimeInput.value = '09:00'; eventEndTimeInput.value = '10:00'; } eventDescriptionInput.value = event.description || ''; eventColorInput.value = event.color || '#4e73df'; eventReminderInput.checked = event.reminder || false; } else { modalTitle.textContent = "Add New Event"; eventIdInput.value = ""; const defaultDate = (currentDate instanceof Date && !isNaN(currentDate)) ? currentDate : new Date(); try { eventDateInput.value = defaultDate.toISOString().split('T')[0]; } catch (e) { eventDateInput.value = new Date().toISOString().split('T')[0]; } eventStartTimeInput.value = '09:00'; eventEndTimeInput.value = '10:00'; eventColorInput.value = '#4e73df'; eventReminderInput.checked = false; } eventModal.style.display = 'flex';
    }
    function openEventModalForTime(date, hour) {
        openEventModal(); try { eventDateInput.value = date.toISOString().split('T')[0]; } catch (e) { eventDateInput.value = new Date().toISOString().split('T')[0]; } const startTime = `${hour.toString().padStart(2, '0')}:00`; const endTime = `${(hour + 1 > 23 ? 23 : hour + 1).toString().padStart(2, '0')}:${hour + 1 > 23 ? '59' : '00'}`; eventStartTimeInput.value = startTime; eventEndTimeInput.value = endTime;
    }
    function closeModals() {
        if (eventModal) eventModal.style.display = 'none'; if (eventDetailsModal) eventDetailsModal.style.display = 'none'; selectedEventId = null; if (eventForm) eventForm.onsubmit = saveOrUpdateEvent; if (modalTitle) modalTitle.textContent = "Add New Event"; if (eventIdInput) eventIdInput.value = "";
    }

    // *** SỬA LỖI (Patch #4): Lưu thời gian local, KHÔNG DÙNG toISOString() ***
    // Helper mới để tạo chuỗi local
    const toLocalIsoNoZ = (d) => {
        if (!(d instanceof Date) || isNaN(d.getTime())) return "";
        const y = d.getFullYear();
        const m = String(d.getMonth() + 1).padStart(2, '0');
        const day = String(d.getDate()).padStart(2, '0');
        const h = String(d.getHours()).padStart(2, '0');
        const min = String(d.getMinutes()).padStart(2, '0');
        const sec = String(d.getSeconds()).padStart(2, '0');
        return `${y}-${m}-${day}T${h}:${min}:${sec}`;
    };

    function saveOrUpdateEvent(e) {
        e.preventDefault(); if (!eventForm) return; const eventId = eventIdInput.value; const dateValue = eventDateInput.value; const startTimeValue = eventStartTimeInput.value; const endTimeValue = eventEndTimeInput.value; if (!dateValue || !startTimeValue || !endTimeValue || !eventTitleInput.value.trim()) { alert("Please fill in title, date, start time, and end time."); return; } let startDateTime, endDateTime; try { startDateTime = new Date(`${dateValue}T${startTimeValue}`); endDateTime = new Date(`${dateValue}T${endTimeValue}`); if (isNaN(startDateTime.getTime()) || isNaN(endDateTime.getTime())) throw new Error("Invalid date/time"); } catch (error) { alert("Invalid date or time format."); return; } if (endDateTime <= startDateTime) { alert("End time must be after start time."); return; }

        // *** SỬA LỖI (Patch #4): Sử dụng helper toLocalIsoNoZ ***
        const eventData = {
            id: eventId || Date.now().toString(),
            title: eventTitleInput.value.trim(),
            startTime: toLocalIsoNoZ(startDateTime), // <— dùng chuỗi local
            endTime: toLocalIsoNoZ(endDateTime),   // <— dùng chuỗi local
            description: eventDescriptionInput.value.trim(),
            color: eventColorInput.value,
            reminder: eventReminderInput.checked
        };

        if (eventId) { const index = events.findIndex(ev => ev.id === eventId); if (index > -1) events[index] = eventData; } else { events.push(eventData); }

        saveEventsToStorage(); // Lưu chuỗi local mới
        renderCalendar();
        renderEventsList();
        closeModals();
        if (eventData.reminder) setReminder(eventData);
    }
    function showEventDetails(eventId) {
        const event = events.find(e => e.id === eventId); if (!event || !eventDetailsModal) return; selectedEventId = eventId; detailsTitle.textContent = event.title; let startDate, endDate; try { startDate = new Date(event.startTime); endDate = new Date(event.endTime); detailsDate.textContent = startDate.toLocaleDateString('en-US', { weekday: 'long', month: 'long', day: 'numeric', year: 'numeric' }); detailsTime.textContent = `${formatTime(startDate)} - ${formatTime(endDate)}`; } catch (e) { detailsDate.textContent = "Invalid date"; detailsTime.textContent = "Invalid time"; } detailsDescription.textContent = event.description || 'No description provided.'; eventDetailsModal.style.display = 'flex';
    }
    function deleteEvent() {
        if (!selectedEventId) return; const eventToDelete = events.find(e => e.id === selectedEventId); if (!eventToDelete) return; if (confirm(`Are you sure you want to delete the event "${eventToDelete.title}"?`)) { events = events.filter(event => event.id !== selectedEventId); saveEventsToStorage(); renderCalendar(); renderEventsList(); closeModals(); selectedEventId = null; }
    }
    function saveEventsToStorage() {
        try { localStorage.setItem('events', JSON.stringify(events)); console.log("Events saved to localStorage."); } catch (error) { console.error("Error saving events to localStorage:", error); }
    }
    function setReminder(event) {
        if (!event || !event.startTime || !event.reminder) return; let reminderTime; try { reminderTime = new Date(event.startTime); reminderTime.setMinutes(reminderTime.getMinutes() - 15); } catch (e) { return; } const now = new Date(); const timeUntilReminder = reminderTime.getTime() - now.getTime(); if (timeUntilReminder > 0) { setTimeout(() => { showReminderNotification(event); }, timeUntilReminder); }
    }
    function showReminderNotification(event) {
        if (!('Notification' in window)) return; const notify = () => { new Notification(`Reminder: ${event.title}`, { body: `Starts at ${formatTime(new Date(event.startTime))}. ${event.description || ''}`, icon: 'https: //cdn-icons-png.flaticon.com/512/3652/3652191.png' }); }; if (Notification.permission === 'granted') notify(); else if (Notification.permission !== 'denied') { Notification.requestPermission().then(permission => { if (permission === 'granted') notify(); }); }
    }
    function requestNotificationPermission() {
        if ('Notification' in window && Notification.permission !== 'granted' && Notification.permission !== 'denied') { /* Notification.requestPermission(); */ }
    }
    function formatTime(date) {
        if (!date || !(date instanceof Date) || isNaN(date.getTime())) return 'Invalid Time'; return date.toLocaleTimeString('en-US', { hour: 'numeric', minute: '2-digit', hour12: true });
    }
    function formatDateTime(startDate, endDate) {
        if (!startDate || !endDate || !(startDate instanceof Date) || !(endDate instanceof Date) || isNaN(startDate.getTime()) || isNaN(endDate.getTime())) return 'Invalid Date Range'; const startOptions = { month: 'short', day: 'numeric' }; const endOptions = { month: 'short', day: 'numeric' }; const isSameDay = startDate.toDateString() === endDate.toDateString(); if (isSameDay) { return `${startDate.toLocaleDateString('en-US', startOptions)} • ${formatTime(startDate)} - ${formatTime(endDate)}`; } else { return `${startDate.toLocaleDateString('en-US', startOptions)} ${formatTime(startDate)} - ${endDate.toLocaleDateString('en-US', endOptions)} ${formatTime(endDate)}`; }
    }

    // --- Start the application ---
    init();

}); // End DOMContentLoaded
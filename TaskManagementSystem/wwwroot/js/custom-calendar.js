// Custom Calendar Implementation for ASP.NET Core MVC
document.addEventListener('DOMContentLoaded', function () {
    // Initialize all date inputs with the custom calendar
    initializeCustomCalendars();
});

function initializeCustomCalendars() {
    // Find all date inputs
    const dateInputs = document.querySelectorAll('input[type="date"]');
    
    dateInputs.forEach(function(originalInput) {
        // Skip if already processed
        if (originalInput.dataset.calendarInitialized === 'true') return;
        originalInput.dataset.calendarInitialized = 'true';
        
        // Hide the original input
        originalInput.style.display = 'none';
        
        // Generate a unique ID if none exists
        if (!originalInput.id) {
            originalInput.id = 'date-' + Math.random().toString(36).substring(2, 9);
        }
        
        // Create a container for our custom date picker
        const container = document.createElement('div');
        container.className = 'position-relative';
        originalInput.parentNode.insertBefore(container, originalInput);
        container.appendChild(originalInput);
        
        // Create a display input (text field that shows the formatted date)
        const displayInput = document.createElement('input');
        displayInput.type = 'text';
        displayInput.className = originalInput.className;
        displayInput.required = originalInput.required;
        displayInput.placeholder = 'Select a date';
        displayInput.readOnly = true;
        
        // Set initial value if original input has value
        if (originalInput.value) {
            const initialDate = new Date(originalInput.value);
            if (!isNaN(initialDate.getTime())) {
                displayInput.value = formatDate(initialDate);
            }
        }
        
        container.appendChild(displayInput);
        
        // Create the calendar container
        const calendarDiv = document.createElement('div');
        calendarDiv.className = 'calendar-container';
        calendarDiv.style.display = 'none';
        container.appendChild(calendarDiv);
        
        // Create the calendar
        renderCalendar(calendarDiv, originalInput, displayInput);
        
        // Show/hide calendar on display input click
        displayInput.addEventListener('click', function (e) {
            e.preventDefault();
            e.stopPropagation();
            
            // Toggle the calendar
            if (calendarDiv.style.display === 'none') {
                // Hide all other open calendars first
                document.querySelectorAll('.calendar-container').forEach(function(cal) {
                    if (cal !== calendarDiv) {
                        cal.style.display = 'none';
                    }
                });
                
                // Show this calendar
                calendarDiv.style.display = 'block';
                
                // Position check - make sure calendar stays in viewport
                const rect = calendarDiv.getBoundingClientRect();
                const viewportHeight = window.innerHeight;
                
                // If calendar extends beyond viewport bottom, position it above the input
                if (rect.bottom > viewportHeight) {
                    calendarDiv.classList.add('position-top');
                } else {
                    calendarDiv.classList.remove('position-top');
                }
                
                // Re-render the calendar to make sure it's up to date
                renderCalendar(calendarDiv, originalInput, displayInput);
            } else {
                calendarDiv.style.display = 'none';
            }
        });
        
        // Close calendar when clicking outside
        document.addEventListener('click', function (e) {
            if (!container.contains(e.target)) {
                calendarDiv.style.display = 'none';
            }
        });
    });
}

function toggleCalendar(calendarDiv) {
    if (calendarDiv.style.display === 'none') {
        calendarDiv.style.display = 'block';
    } else {
        calendarDiv.style.display = 'none';
    }
}

function renderCalendar(container, originalInput, displayInput, customDate = null) {
    // Clear any existing content
    container.innerHTML = '';
    
    // Get current date or the date from the input or the custom date
    let currentDate;
    if (customDate) {
        currentDate = new Date(customDate);
    } else if (originalInput.value) {
        currentDate = new Date(originalInput.value);
    } else {
        currentDate = new Date();
    }
    
    // Ensure we have a valid date
    if (isNaN(currentDate.getTime())) {
        currentDate = new Date();
    }
    
    // Save these in container's dataset for month navigation
    container.dataset.year = currentDate.getFullYear();
    container.dataset.month = currentDate.getMonth();
    
    // Create custom calendar
    const calendarEl = document.createElement('div');
    calendarEl.className = 'custom-calendar';
    container.appendChild(calendarEl);
    
    // Create the calendar header
    const header = document.createElement('div');
    header.className = 'calendar-header';
    
    // Previous month button
    const prevBtn = document.createElement('button');
    prevBtn.className = 'month-nav';
    prevBtn.innerHTML = '&laquo;';
    prevBtn.type = 'button'; // Prevent form submission
    prevBtn.setAttribute('data-action', 'prev-month');
    
    // Next month button
    const nextBtn = document.createElement('button');
    nextBtn.className = 'month-nav';
    nextBtn.innerHTML = '&raquo;';
    nextBtn.type = 'button'; // Prevent form submission
    nextBtn.setAttribute('data-action', 'next-month');
    
    // Month/Year display
    const monthDisplay = document.createElement('div');
    monthDisplay.className = 'current-month';
    monthDisplay.textContent = formatMonthYear(currentDate);
    
    // Assemble header
    header.appendChild(prevBtn);
    header.appendChild(monthDisplay);
    header.appendChild(nextBtn);
    calendarEl.appendChild(header);
    
    // Add click event for navigation
    header.addEventListener('click', function(e) {
        if (e.target.matches('button[data-action="prev-month"]')) {
            e.preventDefault();
            e.stopPropagation();
            navigateMonth(container, originalInput, displayInput, -1);
        } else if (e.target.matches('button[data-action="next-month"]')) {
            e.preventDefault();
            e.stopPropagation();
            navigateMonth(container, originalInput, displayInput, 1);
        }
    });
    
    // Create table
    const table = document.createElement('table');
    
    // Create header row with day names
    const thead = document.createElement('thead');
    const headerRow = document.createElement('tr');
    
    // Add week number header
    const weekNumHeader = document.createElement('th');
    weekNumHeader.className = 'week-number';
    weekNumHeader.textContent = '#';
    headerRow.appendChild(weekNumHeader);
    
    // Add day name headers
    const days = ['M', 'T', 'W', 'T', 'F', 'S', 'S'];
    days.forEach(function(day) {
        const th = document.createElement('th');
        th.textContent = day;
        headerRow.appendChild(th);
    });
    
    thead.appendChild(headerRow);
    table.appendChild(thead);
    
    // Create body
    const tbody = document.createElement('tbody');
    
    // Get today's date for highlighting
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    
    // Get first day of month (always 1st)
    const firstDay = new Date(currentDate.getFullYear(), currentDate.getMonth(), 1);
    
    // Get day of week for the 1st (0-6, 0 = Sunday)
    // Convert to Monday-based (0 = Monday, 6 = Sunday)
    let firstDayIndex = firstDay.getDay() - 1;
    if (firstDayIndex < 0) firstDayIndex = 6; // Handle Sunday
    
    // Calculate the first date to display
    const firstDateToDisplay = new Date(firstDay);
    firstDateToDisplay.setDate(firstDateToDisplay.getDate() - firstDayIndex);
    
    // Get selected date from input if any
    let selectedDate = null;
    if (originalInput.value) {
        selectedDate = new Date(originalInput.value);
        if (isNaN(selectedDate.getTime())) {
            selectedDate = null;
        }
    }
    
    // Current date to display
    let dateToDisplay = new Date(firstDateToDisplay);
    
    // Generate 6 weeks of calendar
    for (let week = 0; week < 6; week++) {
        const row = document.createElement('tr');
        
        // Add week number cell
        const weekNumCell = document.createElement('td');
        weekNumCell.className = 'week-number';
        weekNumCell.textContent = getWeekNumber(dateToDisplay);
        row.appendChild(weekNumCell);
        
        // Add 7 days per week
        for (let day = 0; day < 7; day++) {
            const cell = document.createElement('td');
            
            const daySpan = document.createElement('span');
            daySpan.textContent = dateToDisplay.getDate();
            daySpan.className = 'day';
            
            // Check if day is from current month
            const isCurrentMonth = dateToDisplay.getMonth() === currentDate.getMonth();
            if (!isCurrentMonth) {
                daySpan.classList.add('disabled');
            }
            
            // Check if day is today
            if (isSameDay(dateToDisplay, today)) {
                daySpan.classList.add('today');
            }
            
            // Check if day is selected
            if (selectedDate && isSameDay(dateToDisplay, selectedDate)) {
                daySpan.classList.add('selected');
            }
            
            // Store the date for the click handler
            const clickDate = new Date(dateToDisplay);
            
            // Add day selection handler
            if (isCurrentMonth) {
                daySpan.addEventListener('click', function(e) {
                    e.preventDefault();
                    e.stopPropagation();
                    
                    // Remove selected class from all days first
                    const allSelected = container.querySelectorAll('.day.selected');
                    allSelected.forEach(function(el) {
                        el.classList.remove('selected');
                    });
                    
                    // Add selected class to this day
                    daySpan.classList.add('selected');
                    
                    // Update the original input with ISO date
                    const year = clickDate.getFullYear();
                    const month = String(clickDate.getMonth() + 1).padStart(2, '0');
                    const day = String(clickDate.getDate()).padStart(2, '0');
                    originalInput.value = `${year}-${month}-${day}`;
                    
                    // Update display input with formatted date
                    displayInput.value = formatDate(clickDate);
                    
                    // Hide calendar
                    container.style.display = 'none';
                    
                    // Trigger change event on original input
                    const event = new Event('change', { bubbles: true });
                    originalInput.dispatchEvent(event);
                });
            }
            
            cell.appendChild(daySpan);
            row.appendChild(cell);
            
            // Move to next day
            dateToDisplay.setDate(dateToDisplay.getDate() + 1);
        }
        
        tbody.appendChild(row);
    }
    
    table.appendChild(tbody);
    calendarEl.appendChild(table);
}

function formatDate(date) {
    const options = { year: 'numeric', month: 'long', day: 'numeric' };
    return date.toLocaleDateString('en-US', options);
}

function formatMonthYear(date) {
    const options = { year: 'numeric', month: 'long' };
    return date.toLocaleDateString('en-US', options);
}

function isSameDay(date1, date2) {
    return date1.getDate() === date2.getDate() &&
           date1.getMonth() === date2.getMonth() &&
           date1.getFullYear() === date2.getFullYear();
}

function getWeekNumber(date) {
    const tempDate = new Date(date);
    tempDate.setHours(0, 0, 0, 0);
    tempDate.setDate(tempDate.getDate() + 3 - (tempDate.getDay() + 6) % 7);
    const week1 = new Date(tempDate.getFullYear(), 0, 4);
    return 1 + Math.round(((tempDate - week1) / 86400000 - 3 + (week1.getDay() + 6) % 7) / 7);
}

// Add a new function for month navigation
function navigateMonth(container, originalInput, displayInput, direction) {
    // Get current year and month from container's dataset
    let year = parseInt(container.dataset.year);
    let month = parseInt(container.dataset.month);
    
    // Update month (add direction value: 1 for next, -1 for previous)
    month += direction;
    
    // Handle year change if necessary
    if (month < 0) {
        month = 11;
        year--;
    } else if (month > 11) {
        month = 0;
        year++;
    }
    
    // Update container's dataset
    container.dataset.year = year;
    container.dataset.month = month;
    
    // Create a new date object
    const newDate = new Date(year, month, 1);
    
    // Re-render calendar
    renderCalendar(container, originalInput, displayInput, newDate);
} 
import truncHtml from "trunc-html";

export function formateDateTime(dateStr) {
    // Handles two formats:
    // 1. "11/15/2025 13:32:07" (MM/DD/YYYY HH:mm:ss)
    // 2. "2025-10-05T16:23:30.5420531Z" (ISO 8601)
    
    let date;
    
    // Check if it's the MM/DD/YYYY format
    if (/^\d{1,2}\/\d{1,2}\/\d{4}\s+\d{1,2}:\d{2}:\d{2}$/.test(dateStr)) {
        // Parse MM/DD/YYYY HH:mm:ss format
        const parts = dateStr.split(' ');
        const datePart = parts[0].split('/');
        const timePart = parts[1].split(':');
        
        // Create date: month is 0-indexed in JavaScript Date
        date = new Date(
            parseInt(datePart[2]), // year
            parseInt(datePart[0]) - 1, // month (0-indexed)
            parseInt(datePart[1]), // day
            parseInt(timePart[0]), // hour
            parseInt(timePart[1]), // minute
            parseInt(timePart[2]) // second
        );
    } else {
        // Try parsing as ISO 8601 or other standard formats
        date = new Date(dateStr);
    }
    
    // Check if date is valid
    if (isNaN(date.getTime())) {
        console.warn('Invalid date string:', dateStr);
        return dateStr; // Return original string if parsing fails
    }
    
    return date
        .toLocaleString('en-GB', {
            day: '2-digit',
            month: '2-digit',
            year: '2-digit',
            hour: '2-digit',
            minute: '2-digit',
            hour12: false,
        })
        .replace(',', '')
        .replace(/\//g, '.')
}

export function truncateHtmlText(html, maxLength = 45) {
    // Helper to extract text and keep track of length
    const truncated = truncHtml(html, maxLength)
    return truncated.text;
}

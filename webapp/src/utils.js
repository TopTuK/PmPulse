import truncHtml from "trunc-html";

export function formateDateTime(dateStr) {
    // 2025-10-05T16:23:30.5420531Z => 05.10.25 16:23
    return new Date(dateStr)
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

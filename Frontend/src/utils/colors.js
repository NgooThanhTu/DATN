export const COLOR_PALETTE = [
  '#EF4444', // red
  '#F97316', // orange
  '#F59E0B', // amber
  '#EAB308', // yellow
  '#84CC16', // lime
  '#22C55E', // green
  '#10B981', // emerald
  '#06B6D4', // cyan
  '#3B82F6', // blue
  '#6366F1', // indigo
  '#8B5CF6', // violet
  '#EC4899'  // pink
];

/**
 * Calculates luminance of a hex color and returns a high-contrast text color
 * @param {string} hexColor - The background color in hex format
 * @returns {string} - '#111827' for dark text, '#FFFFFF' for light text
 */
export function getContrastTextColor(hexColor) {
  if (!hexColor) return '#111827';
  
  // Clean hex string
  const hex = hexColor.replace('#', '');
  if (hex.length !== 6 && hex.length !== 3) return '#111827';
  
  // Parse RGB
  let r, g, b;
  if (hex.length === 3) {
    r = parseInt(hex.substring(0, 1) + hex.substring(0, 1), 16);
    g = parseInt(hex.substring(1, 2) + hex.substring(1, 2), 16);
    b = parseInt(hex.substring(2, 3) + hex.substring(2, 3), 16);
  } else {
    r = parseInt(hex.substring(0, 2), 16);
    g = parseInt(hex.substring(2, 4), 16);
    b = parseInt(hex.substring(4, 6), 16);
  }
  
  // Calculate luminance
  const luminance = (0.299 * r + 0.587 * g + 0.114 * b);
  
  // Return dark or light text
  return luminance > 186 ? '#111827' : '#FFFFFF';
}

/**
 * Gets a random color from the predefined palette
 * @param {string} excludeColor - Optional color to avoid picking
 * @returns {string} - A hex color from COLOR_PALETTE
 */
export function getRandomPaletteColor(excludeColor = null) {
  const available = excludeColor ? COLOR_PALETTE.filter(c => c !== excludeColor) : COLOR_PALETTE;
  const pool = available.length > 0 ? available : COLOR_PALETTE;
  return pool[Math.floor(Math.random() * pool.length)];
}

/**
 * Gets a color style object for background and text
 * @param {string} bgColor - The background hex color
 * @returns {Object} - Style object with background and color properties
 */
export function getDynamicColorStyle(bgColor) {
  if (!bgColor || !bgColor.startsWith('#')) {
    bgColor = COLOR_PALETTE[0]; // fallback
  }
  return {
    background: bgColor,
    color: getContrastTextColor(bgColor),
    transition: 'color 0.2s ease, background 0.2s ease'
  };
}

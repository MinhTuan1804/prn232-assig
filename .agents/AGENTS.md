# Project UI/UX & Frontend Guidelines - FlashShop

This project incorporates high-end UI/UX standards for the **FlashShop** E-Commerce & Flash Sales application.

## Active Skills & Guidelines
The following specialized skills are designated as core UI/UX skills for this project:

1. **`ui-ux-pro-max`**:
   - Palette: OLED Dark (`#07070C`), Vantablack cards (`#0D0D16`), Neon Red (`#FF1E27`), Amber Gold (`#FFB800`).
   - Fonts: `Plus Jakarta Sans` for UI/body, `Geist Mono` for countdowns and numeric prices.
   - Glassmorphism & High-Tension Flash Sale components.

2. **`high-end-visual-design`**:
   - Double-Bezel (Doppelrand) nested card architecture (`rounded-[2rem]` outer shell, concentric inner core).
   - Magnetic button physics and button-in-button icon pills for CTAs.
   - Generous macro-whitespace (`py-16` to `py-24`).

3. **`ui-styling`**:
   - Tailwind CSS v4 styling, CSS variables, accessible Radix/shadcn components, dark mode optimizations.

4. **`design-system`**:
   - Consistent 3-tier token architecture for spacing, typography, and color states.

## General Engineering Rules
- Always maintain TypeScript strict safety with zero unused variables or broken JSX tags.
- Preserve responsive fallbacks for viewports below 768px (`w-full`, `px-4`).
- Ensure all real-time inventory updates and countdown timers handle zero states cleanly.

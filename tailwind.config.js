/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./**/*.{html,fs,js}",
    "./src/**/*.{html,fs,js}"
  ],
  theme: {
    extend: {},
  },
  plugins: [require("daisyui")],
  daisyui: {
    themes: ["light", "dark", "cupcake", "corporate"]
  }
}
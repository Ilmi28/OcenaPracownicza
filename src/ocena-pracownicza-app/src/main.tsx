import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App'
import { ThemeProvider } from '@mui/material/styles';
import { theme } from './themes/theme'; // <-- Import motywu

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <ThemeProvider theme={theme}> {/* Dostarczasz motyw */}
      <App />
    </ThemeProvider>
  </StrictMode>,
)

import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App'
import { ThemeProvider } from '@mui/material/styles';
import { theme } from './themes/theme';

// 1. DODAJ IMPORT AUTHPROVIDER
// Upewnij siê, ¿e œcie¿ka './hooks/AuthProvider' jest poprawna wzglêdem tego pliku!
import { AuthProvider } from './hooks/AuthProvider';

createRoot(document.getElementById('root')!).render(
    <StrictMode>
        {/* 2. OWiñ WSZYSTKO W AUTHPROVIDER */}
        <AuthProvider>
            <ThemeProvider theme={theme}>
                <App />
            </ThemeProvider>
        </AuthProvider>
    </StrictMode>,
)
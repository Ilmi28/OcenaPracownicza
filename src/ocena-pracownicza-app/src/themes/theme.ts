import { createTheme } from '@mui/material/styles';

export const theme = createTheme({
  palette: {
    primary: {
      main: '#2D6CDF', 
    },
    secondary: {
      main: '#4B5563', 
    },
    error: {
      main: '#D93025', 
    },
    success: {
      main: '#1AA260',
    },
    background: {
      default: '#F7F7F7',
      paper: '#FFFFFF',
    },
    text: {
      primary: '#1A1A1A',
      secondary: '#6B7280',
    },
  },

  typography: {
    fontFamily: [
      'system-ui',
      'Inter',
      'Avenir',
      'Sans-serif',
    ].join(','),
    h1: { fontWeight: 600 }, 
    h2: { fontWeight: 600 },
    h3: { fontWeight: 600 },
    h4: { fontWeight: 600 },
    h5: { fontWeight: 600 },
    h6: { fontWeight: 600 },
  },

  
  components: {
    MuiButton: {
      styleOverrides: {
        root: {
          borderRadius: 8, 
          textTransform: 'none', 
        },
      },
    },
    MuiOutlinedInput: {
      styleOverrides: {
        notchedOutline: {
          borderColor: '#D1D5DB', 
        },
      },
    },
    MuiPaper: { 
      styleOverrides: {
        root: {
          borderRadius: 8,
          boxShadow: '0px 4px 12px rgba(0, 0, 0, 0.05)',
        },
      },
    },
  },
});
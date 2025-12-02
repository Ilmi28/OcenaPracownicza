import React from 'react';
import { AppBar, Toolbar, Typography, Box } from '@mui/material';
import DashboardIcon from '@mui/icons-material/Dashboard';

interface NavbarProps {
  drawerWidth: number;
}

const Navbar: React.FC<NavbarProps> = ({ drawerWidth }) => {
  return (
    <AppBar
      position="fixed"
      elevation={0}
      sx={{
        width: '100%',
        bgcolor: 'background.paper',
        borderBottom: '1px solid #D1D5DB',
        zIndex: (theme) => theme.zIndex.drawer + 1,
      }}
    >
      <Toolbar disableGutters>
        <Box
          sx={{
            width: drawerWidth,
            display: 'flex',
            alignItems: 'center',
            height: '100%',
            px: 2,
            borderRight: '1px solid #D1D5DB',
          }}
        >
          <DashboardIcon color="primary" sx={{ mr: 1, fontSize: 24 }} />
          <Typography 
            variant="h6" 
            noWrap 
            component="div" 
            color="text.primary" 
            fontWeight="600"
          >
            Ocena Pracownicza
          </Typography>
        </Box>

        <Box 
          sx={{ 
            flexGrow: 1, 
            display: 'flex', 
            justifyContent: 'flex-end', 
            alignItems: 'center', 
            px: 2 
          }}
        >
          <Typography variant="body1" color="text.primary">
            Użytkownik
          </Typography>
        </Box>
      </Toolbar>
    </AppBar>
  );
};
export default Navbar;
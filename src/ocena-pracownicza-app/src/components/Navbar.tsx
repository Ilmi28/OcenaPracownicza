import React from 'react';
import { AppBar, Toolbar, Typography, Box, Button, Avatar } from '@mui/material';
import DashboardIcon from '@mui/icons-material/Dashboard';

interface User {
  firstName: string;
  lastName: string;
  role: string;
}

interface NavbarProps {
  drawerWidth: number;
  user: User | null;
}

const Navbar: React.FC<NavbarProps> = ({ drawerWidth, user }) => {
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
          {!user && (
            <Button 
              variant="contained" 
              color="primary"
              onClick={() => console.log("Logowanie...")}
            >
              Zaloguj się
            </Button>
          )}

          {user && (
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
              <Avatar>
                {user.firstName[0]}
                {user.lastName[0]}
              </Avatar>

              <Box sx={{ textAlign: 'right' }}>
                <Typography fontWeight={600}>
                  {user.firstName} {user.lastName}
                </Typography>
                <Typography variant="body2" color="text.secondary">
                  {user.role}
                </Typography>
              </Box>
            </Box>
          )}
        </Box>
      </Toolbar>
    </AppBar>
  );
};

export default Navbar;

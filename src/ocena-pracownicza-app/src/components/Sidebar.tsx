import React from 'react';
import { Drawer, Toolbar, List, Box } from '@mui/material';
import SidebarItem from './SidebarItem';
import HomeIcon from '@mui/icons-material/Home';
import PeopleIcon from '@mui/icons-material/People';
import PersonIcon from '@mui/icons-material/Person';
import SettingsIcon from '@mui/icons-material/Settings';

const DRAWER_WIDTH = 240;

const Sidebar: React.FC<{ drawerWidth: number }> = ({ drawerWidth }) => {
  return (
    <Drawer
      variant="permanent"
      sx={{
        width: drawerWidth,
        flexShrink: 0,
        '& .MuiDrawer-paper': {
          width: drawerWidth,
          boxSizing: 'border-box',
          bgcolor: 'background.paper', 
          borderRight: '1px solid #D1D5DB', 
        },
      }}
    >
      <Toolbar /> 
      <Box sx={{ overflow: 'auto', p: 2 }}>
        <List>
          <SidebarItem text="Dashboard" to="/" IconComponent={HomeIcon} />
          <SidebarItem text="Profil" to="/profile" IconComponent={PersonIcon} />
          <SidebarItem text="Użytkownicy" to="/users" IconComponent={PeopleIcon} />
          <SidebarItem text="Ustawienia" to="/settings" IconComponent={SettingsIcon} />
        </List>
      </Box>
    </Drawer>
  );
};
export default Sidebar;
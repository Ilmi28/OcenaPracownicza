import { ListItem, ListItemButton, ListItemIcon, ListItemText } from '@mui/material';
import { Link, useLocation } from 'react-router-dom';

interface SidebarItemProps {
  text: string;
  to: string;
  IconComponent: React.ElementType;
}

const SidebarItem: React.FC<SidebarItemProps> = ({ text, to, IconComponent }) => {
  const location = useLocation();
  const isActive = location.pathname === to; 

  return (
    <ListItem disablePadding sx={{ mb: 1 }}>
      <ListItemButton
        component={Link}
        to={to}
        sx={{
          borderRadius: '8px',
          minHeight: 48,
          py: 0.5,
          color: isActive ? 'white' : 'text.primary',
          bgcolor: isActive ? 'primary.main' : 'transparent', 
          '&:hover': {
            bgcolor: isActive ? 'primary.dark' : '#EEEEEE',
            color: isActive ? 'white' : 'text.primary',
          },
        }}
      >
        <ListItemIcon sx={{ color: isActive ? 'white' : 'text.secondary' }}>
          <IconComponent />
        </ListItemIcon>
        <ListItemText 
          primary={text} 
          primaryTypographyProps={{ 
            fontWeight: isActive ? '600' : '400', 
            fontSize: '0.9rem' 
          }} 
        />
      </ListItemButton>
    </ListItem>
  );
};

export default SidebarItem;
import { ListItem, ListItemButton, ListItemIcon, ListItemText } from "@mui/material";
import { Link, useLocation } from "react-router-dom";

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
                    borderRadius: 2.5,
                    minHeight: 46,
                    px: 1.25,
                    py: 0.75,
                    color: isActive ? "primary.main" : "text.primary",
                    bgcolor: isActive ? "rgba(37, 99, 235, 0.10)" : "transparent",
                    border: isActive ? "1px solid rgba(37, 99, 235, 0.24)" : "1px solid transparent",
                    "&:hover": {
                        bgcolor: isActive ? "rgba(37, 99, 235, 0.14)" : "#f1f5f9",
                    },
                }}
            >
                <ListItemIcon
                    sx={{
                        minWidth: 34,
                        color: isActive ? "primary.main" : "text.secondary",
                    }}
                >
                    <IconComponent />
                </ListItemIcon>
                <ListItemText
                    primary={text}
                    primaryTypographyProps={{
                        fontWeight: isActive ? 700 : 500,
                        fontSize: "0.92rem",
                    }}
                />
            </ListItemButton>
        </ListItem>
    );
};

export default SidebarItem;

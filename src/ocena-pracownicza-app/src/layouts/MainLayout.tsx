import { Outlet } from "react-router-dom";
import { Box, CssBaseline, Container, Toolbar } from "@mui/material";

import Navbar from "../components/Navbar";
import Sidebar from "../components/Sidebar";
import Footer from "../components/Footer";
import { useAuth } from "../hooks/AuthProvider";

const DRAWER_WIDTH = 240;

const MainLayout = () => {
    const { user } = useAuth();

    return (
        <Box
            sx={{
                display: "flex",
                flexDirection: "column",
                minHeight: "100vh",
            }}
        >
            <CssBaseline />
            <Navbar drawerWidth={DRAWER_WIDTH} user={user} />

            <Box sx={{ display: "flex", flexGrow: 1 }}>
                <Sidebar drawerWidth={DRAWER_WIDTH} />

                <Box
                    component="main"
                    sx={{
                        flexGrow: 1,
                        bgcolor: "background.default",
                        p: 3,
                        width: `calc(100% - ${DRAWER_WIDTH}px)`,
                    }}
                >
                    <Toolbar />

                    <Container maxWidth="lg" sx={{ flexGrow: 1, mb: 3 }}>
                        <Outlet />
                    </Container>

                    <Footer />
                </Box>
            </Box>
        </Box>
    );
};

export default MainLayout;

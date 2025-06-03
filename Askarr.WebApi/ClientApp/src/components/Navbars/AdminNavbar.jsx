import React, { useState, useContext } from "react";
import { Link } from "react-router-dom";
import { useDispatch } from "react-redux";
import {
  Navbar,
  Container,
  Nav,
  NavbarToggler,
  Collapse,
  NavItem
} from "reactstrap";
import { ThemeContext } from "../../contexts/ThemeContext";
import DarkModeToggle from "../ThemeToggle/DarkModeToggle";
import { logout } from "../../store/actions/UserActions";

function AdminNavbar({ brandText }) {
  const [isOpen, setIsOpen] = useState(false);
  const { darkMode } = useContext(ThemeContext);
  const dispatch = useDispatch();

  const toggle = () => setIsOpen(!isOpen);

  const handleLogout = (e) => {
    e.preventDefault();
    dispatch(logout());
  };

  return (
    <>
      <Navbar 
        className={`navbar-top ${darkMode ? 'navbar-dark bg-dark' : 'navbar-light bg-white'} shadow`} 
        expand="md" 
        id="navbar-main"
      >
        <Container fluid>
          <Link
            className={`h4 mb-0 ${darkMode ? 'text-white' : 'text-dark'} text-uppercase d-none d-lg-inline-block`}
            to="/"
          >
            {brandText}
          </Link>
          <NavbarToggler onClick={toggle} />
          <Collapse isOpen={isOpen} navbar>
            <Nav className="ml-auto align-items-center" navbar>
              <NavItem className="d-flex align-items-center">
                <DarkModeToggle />
              </NavItem>
            </Nav>
          </Collapse>
        </Container>
      </Navbar>
    </>
  );
}

export default AdminNavbar; 
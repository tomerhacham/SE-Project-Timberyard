import { render, screen } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { MemoryRouter } from "react-router-dom";
import Login from "../components/login/Login"
import AuthProvider from "../contexts/AuthContext";

describe('LOGIN TESTS', () => {
    test('Login button is enabled after both email address & password are entered.', () => {
        render(
            <AuthProvider>
                <MemoryRouter initialEntries={["/login"]}>
                    <Login />
                </MemoryRouter>
            </AuthProvider>
        );

        expect(screen.getByRole('button', {name: /sign in/i})).toBeDisabled();
        
        userEvent.type(screen.getByLabelText(/email address/i), "admin@ribbon.com");
        expect(screen.getByRole('button', {name: /sign in/i})).toBeDisabled();
        
        userEvent.type(screen.getByLabelText(/password/i), "123456");
        expect(screen.getByRole('button', {name: /sign in/i})).toBeEnabled();
    });

})
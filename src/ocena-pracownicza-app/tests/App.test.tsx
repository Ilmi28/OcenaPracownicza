import { describe, it, expect, vi } from "vitest";
import { render } from "@testing-library/react";
import App from "../src/App";

describe("App component", () => {
    it("renders without crashing and logs no errors", () => {
        const consoleError = vi
            .spyOn(console, "error")
            .mockImplementation(() => {});

        const { container } = render(<App />);
        expect(container).toBeTruthy();

        expect(consoleError).not.toHaveBeenCalled();

        consoleError.mockRestore();
    });
});

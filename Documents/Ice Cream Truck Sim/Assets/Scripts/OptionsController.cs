using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsController : MonoBehaviour 
{
	public Dropdown mapSizeDropdown;
	public Slider volume;
	public Slider mouseSensitivity;
	public Dropdown resolutionDropdown;
	public Slider screenBrightness;

	OptionsData options;

	List<int> mapSizeOptions;

	void Start()
	{
		options = XMLController.Instance.Options;

		if (options == null)
			options = new OptionsData();

		mapSizeOptions = new List<int>{ 5, 10,  15, 25};

		if (mapSizeOptions.Contains(options.mapSize))
			mapSizeDropdown.value = mapSizeOptions.FindIndex(x => x == options.mapSize);
		else if (options.mapSize < 100 && options.mapSize > 1) //If the player finds and edits the save file, I'll allow different map sizes but limit them to less than 100, more could crash their computer.
			mapSizeOptions.Add(options.mapSize);
		else
			options.mapSize = mapSizeOptions[0];

		mapSizeDropdown.AddOptions(mapSizeOptions.ConvertAll(x => x.ToString()));
		mapSizeDropdown.value = mapSizeOptions.FindIndex(x => x == options.mapSize);
		
		volume.value = options.volume;
		mouseSensitivity.value = options.mouseSensitivity;
		screenBrightness.value = options.screenBrightness;

		int resolutionIndex = -1;
		foreach (Resolution r in Screen.resolutions)
		{
			string name = r.width + " X " + r.height;
			Dropdown.OptionData option = new Dropdown.OptionData(name);

			//Check for duplicates, because there should be some with different refresh rates.
			if (resolutionDropdown.options.Find(x => x.text == name) == null)
				resolutionDropdown.options.Add(option);
			else
				continue;

			if (r.width == options.screenResolution.width && r.height == options.screenResolution.height)
				resolutionIndex = resolutionDropdown.options.Count - 1;
		}

		//If the current resolution is not on the list for some reason, add it.
		if (resolutionIndex == -1)
		{
			resolutionDropdown.options.Add(new Dropdown.OptionData(options.screenResolution.width + " X " + options.screenResolution.height));
			resolutionIndex = resolutionDropdown.options.Count - 1;
		}

		resolutionDropdown.value = resolutionIndex;
	}

	public void OnMapSizeChange(int optionIndex)
	{
		options.mapSize = mapSizeOptions[optionIndex];
	}

	public void OnVolumeChange(float vol)
	{
		options.volume = vol;
	}

	public void OnMouseSensitivityChange(float sensitivity)
	{
		options.mouseSensitivity = sensitivity;
	}

	public void OnFullscreenChange(bool isOn)
	{
		Screen.fullScreen = isOn;
		options.fullscreen = isOn;
	}
	public void OnResolutionChange(int resolutionIndex)
	{
		Resolution selected = Screen.resolutions[resolutionIndex];
		Screen.SetResolution(selected.width, selected.height, options.fullscreen);

		options.screenResolution = selected;
	}

	public void OnBrightnessChange(float value)
	{
		options.screenBrightness = value;
	}

	public void OnOptionsExit()
	{
		if (Camera.main.GetComponent<ThirdPersonCamera>()) //ToDo: Find a cheaper and cleaner way to do this.
			Camera.main.GetComponent<ThirdPersonCamera>().ChangeMouseSensitivity(options.mouseSensitivity);

		XMLController.Instance.Options = options;
		XMLController.Instance.Save();
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsController : MonoBehaviour 
{
	public Dropdown mapSizeDropdown;
	public Slider volume;
	public Slider mouseSensitivity;

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
		else if (options.mapSize < 100 && options.mapSize > 1) //If the player finds and edits the save file, I'll allow different map sizes but limit them to less than 100, more could crash their computor.
			mapSizeOptions.Add(options.mapSize);
		else
			options.mapSize = mapSizeOptions[0];

		mapSizeDropdown.AddOptions(mapSizeOptions.ConvertAll(x => x.ToString()));
		mapSizeDropdown.value = mapSizeOptions.FindIndex(x => x == options.mapSize);
		
		volume.value = options.volume;
		mouseSensitivity.value = options.mouseSensitivity;
	}

	public void OnMapSizeChange(int optionIndex)
	{
		options.mapSize = mapSizeOptions[optionIndex];
	}

	public void OnOptionsExit()
	{
		XMLController.Instance.Options = options;

		XMLController.Instance.Save();
	}
}
